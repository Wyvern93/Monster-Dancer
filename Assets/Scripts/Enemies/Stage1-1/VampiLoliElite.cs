using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class VampiLoliElite : Enemy
{
    int beatCD;
    private Vector3 targetPos;
    private int phase;

    private List<Vector2> anchors = new List<Vector2>()
    {
        new Vector2(-2.75f, 0),
        new Vector2(2.75f, 0),
        new Vector2(0,0),
        new Vector2(-2.75f, -4.75f),
        new Vector2(2.75f, -4.75f),
        new Vector2(0,-4.75f),
        new Vector2(-2.75f, -2.5f),
        new Vector2(2.75f, -2.5f)
    };
    public override void OnSpawn()
    {
        base.OnSpawn();
        speed = 0.8f;
        beatCD = -1;
        phase = 0;
        facingRight = transform.position.x < Player.instance.transform.position.x;
        UIManager.Instance.PlayerUI.SetBossBarName("Ruri Fang");
        UIManager.Instance.PlayerUI.ShowBossBar(true);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
    }

  

    protected override void OnBeat()
    {
        switch (phase)
        {
            case 0:
                DistantShoot();
                break;
            case 1:
                SpearPhase();
                break;
            case 2:
                BatPhase();
                break;
        }
    }

    private void DistantShoot()
    {
        canBeKnocked = false;
        beatCD++;
        if (beatCD == 0 || beatCD == 4 || beatCD == 8)
        {
            StartCoroutine(ShootBlood());
        }
        else
        {
            FindFurtherAnchor();
            if (CanMove() && !isAttacking) StartCoroutine(MoveToTarget());
        }
        if (beatCD >= 12)
        {
            beatCD = -1;
            phase = 1;
        }
    }

    private void SpearPhase()
    {
        canBeKnocked = false;
        if ((Vector2)transform.position != (Vector2)Camera.main.transform.position + anchors[2])
        {
            targetPos = (Vector2)Camera.main.transform.position + anchors[2];
            if (CanMove()) StartCoroutine(MoveToTarget());
            return;
        }
        beatCD++;
        if (beatCD == 0 || beatCD == 8) StartCoroutine(SpearAttackCoroutine());
        if (beatCD == 4) StartCoroutine(SpearAttackCoroutine(1.5f));
        if (beatCD >= 12)
        {
            beatCD = -1;
            phase = 2;
        }
    }

    private void BatPhase()
    {
        beatCD++;
        if (beatCD == 0)
        {
            TransformEffect();
            speed = 1.5f;
            animator.Play("vampiloli_bat_move");
        }
        if (beatCD < 16)
        {
            StartCoroutine(OrbitPlayer());
        }
        if (beatCD < 16 && beatCD % 2 == 0)
        {
            StartCoroutine(BatShootCoroutine());
        }
        if (beatCD >= 16)
        {
            canBeKnocked = false;
            targetPos = (Vector2)Camera.main.transform.position + anchors[2];
            if ((Vector2)transform.position != (Vector2)Camera.main.transform.position + anchors[2])
            {
                if (CanMove()) StartCoroutine(MoveToTarget(true));
            }
            else
            {
                speed = 0.8f;
                canBeKnocked = true;
                TransformEffect();
                animator.Play("vampiloli_normal");
                beatCD = -1;
                phase = 0;
            }
        }
    }

    private void TransformEffect()
    {
        SmokeExplosion effect = PoolManager.Get<SmokeExplosion>();
        effect.transform.position = transform.position;
    }

    private IEnumerator OrbitPlayer()
    {
        Vector2 center = Player.instance.transform.position;
        float radius = 5f;

        float angle = BulletBase.VectorToAngle(((Vector2)transform.position - center).normalized);

        angle += 45f;

        angle *= Mathf.Deg2Rad;
        targetPos = center + new Vector2(Mathf.Cos (angle) * radius, Mathf.Sin(angle) * radius);

        isMoving = true;

        float time = 0;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * 6);
            if (transform.position == targetPos) break;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);

        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }
    private IEnumerator BatShootCoroutine()
    {
        isAttacking = true;
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 playerdir = Player.instance.GetClosestPlayer(transform.position + (-Vector3.up * 0.4f)) - transform.position;
        playerdir.Normalize();

        float baseAngle = BulletBase.VectorToAngle(playerdir);

        for (int j = -1; j < 2; j ++)
        {
            float angle = baseAngle + (j * 15f);
            Vector2 dir = BulletBase.angleToVector(angle);
            SpawnSmallBloodBullet(dir);
        }

        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);

        bulletSpawnEffect.Despawn();
        isAttacking = false;
        yield break;
    }

    private IEnumerator SpearAttackCoroutine(float offset = 0)
    {
        isAttacking = true;
        animator.Play("vampiloli_preattack");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (!BeatManager.isBeat) yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        float spearOffset = 2.75f;
        for (int i = -2; i < 3; i++)
        {
            BulletBase bullet = PoolManager.Get<BulletBase>();

            bullet.transform.position = Camera.main.transform.position + new Vector3((i * spearOffset) + offset, 3.25f, 60f);
            bullet.direction = Vector2.down;
            bullet.speed = 0;
            bullet.atk = 5;
            bullet.lifetime = 8;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = true;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpeedOverTimeBehaviour() { start = 1, end = -1, speedPerBeat = 20, targetSpeed = 20 }
            };
            bullet.animator.Play("rurispear");
            bullet.enemySource = this;
            bullet.OnSpawn();
        }

        float time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame(); // isStunned!
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        bulletSpawnEffect.Despawn();
        animator.Play("vampiloli_normal");
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        isAttacking = false;
        yield break;
    }

    private IEnumerator ShootBlood()
    {
        isAttacking = true;
        animator.Play("vampiloli_preattack");
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 playerdir = Player.instance.GetClosestPlayer(transform.position + (-Vector3.up * 0.4f)) - transform.position;
        playerdir.Normalize();

        float baseAngle = BulletBase.VectorToAngle(playerdir);

        for (int j = -2; j < 3; j++)
        {
            float angle = baseAngle + (j * 15f);
            Vector2 dir = BulletBase.angleToVector(angle);
            ShootBloodBullet(dir);
        }

        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);

        bulletSpawnEffect.Despawn();
        isAttacking = false;
        animator.Play("vampiloli_normal");
        yield break;
    }

    private void ShootBloodBullet(Vector2 attackDir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3)(attackDir * 0.5f) + (Vector3.up * 0.5f);
        bullet.direction = attackDir;
        bullet.speed = 10;
        bullet.atk = 5;
        bullet.lifetime = 5;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
            };
        bullet.animator.Play("bloodbullet_big");
        bullet.enemySource = this;
        bullet.OnSpawn();
    }

    private void SpawnSmallBloodBullet(Vector2 attackDir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();
        bullet.transform.position = transform.position + (Vector3)(attackDir * 0.5f) + (Vector3.up * 0.5f);
        bullet.direction = attackDir;
        bullet.speed = 12;
        bullet.atk = 5;
        bullet.lifetime = 4;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
            };
        bullet.animator.Play("bloodbullet");
        bullet.enemySource = this;
        bullet.OnSpawn();
    }


    private void FindFurtherAnchor()
    {
        Vector2 playerPos = Camera.main.transform.position - Player.instance.transform.position;

        List<float> distances = new List<float>();
        foreach (Vector2 anchorPos in anchors)
        {
            distances.Add(Vector2.Distance(anchorPos, playerPos));
        }
        float maxDistance = 9999;
        int index = -1;
        for (int i = 0; i < distances.Count; i++)
        {
            if (distances[i] < maxDistance)
            {
                maxDistance = distances[i];
                index = i;
            }
        }
        targetPos = (Vector2)Camera.main.transform.position + anchors[index];
    }

    protected override void OnBehaviourUpdate()
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

    IEnumerator MoveToTarget(bool isBat = false)
    {
        isMoving = true;

        float time = 0;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        if (!isBat) animator.Play(moveAnimation);
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * 6);
            if (transform.position == targetPos) break;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (!isBat) animator.Play(idleAnimation);
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);

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
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play(idleAnimation);
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}