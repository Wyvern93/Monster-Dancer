using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackOElite : Enemy
{
    int beatCD;
    private Vector3 targetPos;
    private int phase;
    [SerializeField] SpriteRenderer shadow;
    [SerializeField] BoxCollider2D boxCollider;
    private bool lightAttackToggle;
    public override void OnSpawn()
    {
        shouldMove = false;
        rb.velocity = Vector3.zero;
        targetPos = (Vector2)Camera.main.transform.position;
        beatCD = -1;
        base.OnSpawn();
        UIManager.Instance.PlayerUI.SetBossBarName("King Jack 'o");
        UIManager.Instance.PlayerUI.ShowBossBar(true);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
        speed = 1.4f;
        canBeKnocked = false;
    }

    public override void MoveUpdate()
    { 
        if (GameManager.isPaused || stunStatus.isStunned())
        {
            velocity = Vector2.zero;
            return;
        }
        if (!shouldMove || !CanMove() || isAttacking)
        {
            velocity = Vector2.zero;
            return;
        }

        facingRight = transform.position.x < targetPos.x;
        velocity = Vector2.zero;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * 3 * Time.deltaTime);
    }

    protected override void OnBeat()
    {
        // Move Behaviour
        if (CanMove())
        {
            if (shouldMove) animator.Play(moveAnimation);
            else animator.Play(idleAnimation);
        }
        switch (phase)
        {
            case 0:
                HandleSpiralPhase();
                break;
            case 1:
                HandleGhostPhase();
                break;
            case 2:
                HandleLightPhase();
                break;
            case 3:
                HandleMoveToSpiral();
                break;
        }

    }

    private void HandleSpiralPhase()
    {
        shouldMove = false;
        canBeKnocked = false;
        beatCD++;
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            ShootSpiral();
        }
        if (beatCD >= 6)
        {
            phase = 1;
            beatCD = -1;
        }
    }

    private void HandleGhostPhase()
    {
        canBeKnocked = true;
        beatCD++;
        if (beatCD < 20)
        {
            if (beatCD == 0 || beatCD == 4 || beatCD == 8 || beatCD == 12 || beatCD == 16)
            {
                shouldMove = false;
                StartCoroutine(HideAndReappear());
            }
            else if(CanMove())
            {
                targetPos = Player.instance.GetClosestPlayer(transform.position);
                shouldMove = true;
                //MoveTowardsPlayer();
            }

        }
        else
        {
            phase = 2;
            beatCD = -1;
        }
        
    }

    private void HandleLightPhase()
    {
        shouldMove = false;
        beatCD++;
        if (beatCD == 0) StartCoroutine(Hide());
        else if (beatCD == 12) StartCoroutine(Show());
        else if (beatCD == 2 || beatCD == 6 || beatCD == 10)
        {
            SpawnLightAttack();
        }
        if (beatCD >= 13)
        {
            phase = 3;
            beatCD = -1;
        }
        
    }

    private void SpawnLightAttack()
    {
        AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
        Vector2 center = (Vector2)Camera.main.transform.position;

        Vector2 start = new Vector2(center.x - 8f, center.y - 4f);
        Vector2 offset = new Vector2(4f, 2.8f);

        bool playedSound = false;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (lightAttackToggle)
                {
                    if ((x + y - 1) % 2 == 0)
                    {
                        LightAttack attack = PoolManager.Get<LightAttack>();
                        attack.transform.position = new Vector2(start.x + (offset.x * x), start.y + (offset.y * y));
                        attack.playSound = false;
                        if (!playedSound)
                        {
                            attack.playSound = true;
                            AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
                            playedSound = true;
                        }
                    }
                }
                else
                {
                    if ((x + y) % 2 == 0)
                    {
                        LightAttack attack = PoolManager.Get<LightAttack>();
                        attack.transform.position = new Vector2(start.x + (offset.x * x), start.y + (offset.y * y));
                        attack.playSound = false;
                        if (!playedSound)
                        {
                            attack.playSound = true;
                            AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
                            playedSound = true;
                        }
                    }
                }
                
            }
        }
        lightAttackToggle = !lightAttackToggle;
    }

    private void HandleMoveToSpiral()
    {
        shouldMove = true;
        canBeKnocked = false;
        if ((Vector2)transform.position != (Vector2)Camera.main.transform.position)
        {
            targetPos = (Vector2)Camera.main.transform.position;
            //if (CanMove()) StartCoroutine(MoveToTarget());
        }
        else
        {
            shouldMove = false;
            phase = 0;
            beatCD = -1;
        }
    }

    private IEnumerator Hide()
    {
        animator.Play("boojr_preattack");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        boxCollider.enabled = false;
        while (Sprite.color.a > 0.05f)
        {
            while (GameManager.isPaused) yield return null;
            Sprite.color = Color.Lerp(Sprite.color, new Color(1, 1, 1, 0), Time.deltaTime * 16f);
            shadow.color = Sprite.color;
            yield return null;
        }
        Sprite.color = new Color(1, 1, 1, 0);
        shadow.color = Sprite.color;
        yield break;
    }

    private IEnumerator Show()
    {
        animator.Play("boojr_preattack");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        while (Sprite.color.a < 0.95f)
        {
            while (GameManager.isPaused) yield return null;
            Sprite.color = Color.Lerp(Sprite.color, new Color(1, 1, 1, 1), Time.deltaTime * 16f);
            shadow.color = Sprite.color;
            yield return null;
        }
        Sprite.color = Color.white;
        shadow.color = Sprite.color;
        isMoving = false;
        animator.Play("boojr_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
        boxCollider.enabled = true;
        yield break;
    }

    private IEnumerator HideAndReappear()
    {
        shouldMove = false;
        isMoving = true;
        animator.Play("boojr_preattack");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        boxCollider.enabled = false;
        while (Sprite.color.a > 0.05f)
        {
            while (GameManager.isPaused) yield return null;
            Sprite.color = Color.Lerp(Sprite.color, new Color(1, 1, 1, 0), Time.deltaTime * 16f);
            shadow.color = Sprite.color;
            yield return null;
        }
        Sprite.color = new Color(1, 1, 1, 0);

        Vector2 cameraPos = Camera.main.transform.position;
        Vector2 targetSpawn = cameraPos;
        targetSpawn = ((Vector2)Player.instance.transform.position + Random.insideUnitCircle.normalized * 6f);
        while (targetSpawn.x > cameraPos.x + 14 || targetSpawn.x < cameraPos.x - 14 || targetSpawn.y > cameraPos.y + 8 || targetSpawn.y < cameraPos.y - 8)
        {
            targetSpawn = ((Vector2)Player.instance.transform.position + Random.insideUnitCircle.normalized * 10f);
        }
        transform.position = targetSpawn;

        while (Sprite.color.a < 0.95f)
        {
            while (GameManager.isPaused) yield return null;
            Sprite.color = Color.Lerp(Sprite.color, new Color(1, 1, 1, 1), Time.deltaTime * 16f);
            shadow.color = Sprite.color;
            yield return null;
        }
        boxCollider.enabled = true;
        Sprite.color = Color.white;
        isMoving = false;
        animator.Play("boojr_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        shouldMove = true;
        yield break;
    }

    public void ShootSpiral()
    {
        StartCoroutine(SpiralCoroutine());
    }

    private IEnumerator SpiralCoroutine()
    {
        shouldMove = false;
        isAttacking = true;
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        animator.Play("boojr_preattack");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        List<BulletBase> tempbullets = new List<BulletBase>();
        float diff = 360f / 12f;
        for (int i = 0; i < 12; i++)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            tempbullets.Add(SpawnBullet(i * diff, 10 + (i % 3), 1 + ((i % 3) / 2f)));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 8f);
        }
        foreach (BulletBase bullet in tempbullets)
        {
            bullet.ResetBeat();
            bullet.frozen = false;
            bullet.beat = 0;
        }
        float time = BeatManager.GetBeatDuration();
        while (time > 0)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;
            time -= Time.deltaTime;
            yield return null;
        }
        bulletSpawnEffect.Despawn();
        isAttacking = false;
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
        animator.Play("boojr_normal");
        shouldMove = true;
        yield break;

    }

    private BulletBase SpawnBullet(float angle, float speed, float dist)
    {
        AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.shootBullet);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + ((Vector3)dir * -dist) + (Vector3.up * 0.5f);// + (Vector3)(dir * -dist) + (Vector3.one * 0.5f);
        bullet.direction = dir;
        bullet.speed = 0;
        bullet.atk = 5;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one * 2f;
        bullet.startOnBeat = false;
        bullet.frozen = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpeedOverTimeBehaviour() { start = 2, end = -1, speedPerBeat = 3f, targetSpeed = speed},
            };
        bullet.animator.Play("ghostbullet");
        bullet.OnSpawn();
        bullet.beat = 0;
        bullet.enemySource = this;
        return bullet;
    }

    protected override void OnBehaviourUpdate()
    {

    }

    protected override void OnInitialize()
    {
    }

    void MoveTowardsPlayer()
    {
        if (isMoving) return;

        Move();

    }

    public void Move()
    {
        StartCoroutine(JumpCoroutine());
    }

    public override bool CanTakeDamage()
    {
        return true;
    }

    IEnumerator MoveToTarget()
    {
        shouldMove = false;
        isMoving = true;

        float time = 0;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * 3);
            if (transform.position == targetPos) break;
            time += Time.deltaTime;
            yield return null;
        }
        animator.Play(idleAnimation);

        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    protected override IEnumerator JumpCoroutine()
    {
        shouldMove = false;
        isMoving = true;

        float time = 0;
        targetPos = Player.instance.transform.position;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            velocity = dir * speed * 3;
            time += Time.deltaTime;
            yield return null;
        }
        animator.Play(idleAnimation);
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        shouldMove = true;
        yield break;
    }
}