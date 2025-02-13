using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackOElite : Enemy
{
    int beatCD;
    private Vector3 targetPos;
    private int phase;
    private float lightAttackAngleBase;
    [SerializeField] SpriteRenderer shadow;
    [SerializeField] BoxCollider2D boxCollider;
    public override void OnSpawn()
    {
        lightAttackAngleBase = 0;
        beatCD = -1;
        base.OnSpawn();
        UIManager.Instance.PlayerUI.SetBossBarName("King Jack 'o");
        UIManager.Instance.PlayerUI.ShowBossBar(true);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
        speed = 1.0f;
        canBeKnocked = false;
    }

    protected override void OnBeat()
    {
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
        if (beatCD < 24)
        {
            if (beatCD == 0 || beatCD == 8 || beatCD == 16)
            {
                StartCoroutine(HideAndReappear());
            }
            else if(CanMove())
            {
                MoveTowardsPlayer();
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
        LightAttack centerAttack = PoolManager.Get<LightAttack>();
        AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
        centerAttack.playSound = true;
        Vector2 center = (Vector2)Camera.main.transform.position;
        centerAttack.transform.position = center;

        float aAngle = lightAttackAngleBase;
        float bAngle = lightAttackAngleBase + 45;
        float cAngle = lightAttackAngleBase + 90;

        float diff = 360f / 5f;

        for (int a = 0; a < 5; a++)
        {
            float angle = (aAngle + (a * diff)) * Mathf.Deg2Rad;
            LightAttack attack = PoolManager.Get<LightAttack>();
            attack.transform.position = new Vector2(center.x + (5 * Mathf.Cos(angle)), center.y + (3f * Mathf.Sin(angle)));
            attack.playSound = false;
        }

        for (int b = 0; b < 5; b++)
        {
            float angle = (bAngle + (b * diff)) * Mathf.Deg2Rad;
            LightAttack attack = PoolManager.Get<LightAttack>();
            attack.transform.position = new Vector2(center.x + (10 * Mathf.Cos(angle)), center.y + (6f * Mathf.Sin(angle)));
            attack.playSound = false;
        }

        for (int c = 0; c < 5; c++)
        {
            float angle = cAngle + ((c * diff)) * Mathf.Deg2Rad;
            LightAttack attack = PoolManager.Get<LightAttack>();
            attack.transform.position = new Vector2(center.x + (15 * Mathf.Cos(angle)), center.y + (9f * Mathf.Sin(angle)));
            attack.playSound = false;
        }

        lightAttackAngleBase += 45f;
    }

    private void HandleMoveToSpiral()
    {
        canBeKnocked = false;
        if ((Vector2)transform.position != (Vector2)Camera.main.transform.position)
        {
            targetPos = (Vector2)Camera.main.transform.position;
            if (CanMove()) StartCoroutine(MoveToTarget());
        }
        else
        {
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
        while (targetSpawn.x > cameraPos.x + 10 || targetSpawn.x < cameraPos.x - 10 || targetSpawn.y > cameraPos.y + 5 || targetSpawn.y < cameraPos.y - 5)
        {
            targetSpawn = ((Vector2)Player.instance.transform.position + Random.insideUnitCircle.normalized * 6f);
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
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
        yield break;
    }

    public void ShootSpiral()
    {
        StartCoroutine(SpiralCoroutine());
    }

    private IEnumerator SpiralCoroutine()
    {
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
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 12f);
        }
        foreach (BulletBase bullet in tempbullets)
        {
            bullet.beat = 1;
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
        yield break;

    }

    private BulletBase SpawnBullet(float angle, float speed, float dist)
    {
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + ((Vector3)dir * -dist) + (Vector3.up * 0.5f);// + (Vector3)(dir * -dist) + (Vector3.one * 0.5f);
        bullet.direction = dir;
        bullet.speed = 0;
        bullet.atk = 5;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one * 2f;
        bullet.startOnBeat = false;
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
        isMoving = true;

        float time = 0;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * 6);
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
        isMoving = true;

        float time = 0;
        targetPos = Player.instance.transform.position;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return null;
        }
        animator.Play(idleAnimation);
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }
}