using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class NekomanderElite : Enemy
{
    int beatCD;
    private Vector3 targetPos;
    private int phase;
    private List<Enemy> enemies;
    [SerializeField] BoxCollider2D boxCollider;
    protected override void OnBeat()
    {
        switch (phase)
        {
            case 0:
                SummonPhase();
                break;
            case 1:
                JumpOnPlayer();
                break;
            case 2:
                ChaseShoot();
                break;
        }
    }

    private void SummonPhase()
    {
        beatCD++;
        if (beatCD == 0)
        {
            StartCoroutine(SummonEnemies());
        }
        if (beatCD >= 6) 
        {
            beatCD = -1;
            phase = 1;
        }
    }

    private IEnumerator SummonEnemies()
    {
        facingRight = transform.position.x < Player.instance.transform.position.x;
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        animator.Play("nekomander_preattack");

        float angleDiff = 360f / 5f;
        float dist = 1.3f;
        List<SmallMagicCircle> list = new List<SmallMagicCircle>();
        for (int i = 0; i < 5; i++)
        {
            float angle = angleDiff * i * Mathf.Deg2Rad;
            SmallMagicCircle magicCircle = PoolManager.Get<SmallMagicCircle>();
            magicCircle.transform.position = transform.position + new Vector3(dist * Mathf.Cos(angle), dist * Mathf.Sin(angle));
            magicCircle.despawnTime = 4;
            list.Add(magicCircle);
        }

        float time = 0;
        while (time <= BeatManager.GetBeatDuration() * 2)
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }
        facingRight = transform.position.x < Player.instance.transform.position.x;
        EnemyGroup enemyGroup = PoolManager.Get<EnemyGroup>();
        enemyGroup.aIType = EnemyAIType.Spread;
        enemyGroup.enemies = new List<Enemy>();
        foreach (SmallMagicCircle magicCircle in list)
        {
            ClawRiff clawriff = PoolManager.Get<ClawRiff>();
            clawriff.transform.position = magicCircle.transform.position;
            clawriff.group = group;
            clawriff.aiType = EnemyAIType.Spread;
            clawriff.SpawnIndex = 0;
            clawriff.circleCollider.enabled = true;
            clawriff.OnSpawn();
            enemies.Add(clawriff);

            SmokeExplosion summonEffect = PoolManager.Get<SmokeExplosion>();
            summonEffect.transform.position = clawriff.transform.position;
            summonEffect.transform.localScale = Vector3.one;
        }
        AudioController.PlaySound(AudioController.instance.sounds.spawn);
        enemyGroup.OnGroupInit();

        time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }
        animator.Play("nekomander_normal");
        yield break;
    }

    private void JumpOnPlayer()
    {
        beatCD++;
        targetPos = Player.instance.transform.position;
        if (beatCD == 0) StartCoroutine(JumpToPlayerCoroutine());
    }

    protected IEnumerator JumpToPlayerCoroutine()
    {
        isMoving = true;

        boxCollider.enabled = false;
        float time = 0;
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play("nekomander_jump");
        velocity = Vector3.zero;
        while (time <= BeatManager.GetBeatDuration() * 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * 6 * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(1f, 0.3f);
        boxCollider.enabled = true;
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        animator.Play("nekomander_normal");

        time = 0;
        while (time <= BeatManager.GetBeatDuration() * 2f)
        {
            while (GameManager.isPaused) yield return null; // isStunned!
            time += Time.deltaTime;
            yield return null;
        }

        beatCD = -1;
        phase = 2;
        yield break;
    }

    private void ChaseShoot()
    {
        beatCD++;
        if (beatCD == 0 || beatCD == 8 || beatCD == 16) // Attack
        {
            StartCoroutine(ConeAttack());
        }
        else
        {
            if (beatCD == 24)
            {
                beatCD = -1;
                phase = 0;
            }
            else if(!isAttacking && CanMove()) // Move
            {
                StartCoroutine(MoveToTarget());
            }

        }
    }

    private IEnumerator ConeAttack()
    {
        isAttacking = true;
        animator.Play("nekomander_preattack");
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (!BeatManager.isBeat) yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 playerdir = Player.instance.GetClosestPlayer(transform.position + (-Vector3.up * 0.4f)) - transform.position;
        playerdir.Normalize();

        float baseAngle = BulletBase.VectorToAngle(playerdir);

        for (int j = -1; j < 2; j++)
        {
            float angle = baseAngle + (j * 15f);
            Vector2 dir = BulletBase.angleToVector(angle);
            ShootBellBullet(dir);
        }

        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);

        bulletSpawnEffect.Despawn();
        isAttacking = false;
        animator.Play("nekomander_normal");
        yield break;
    }

    protected override void OnBehaviourUpdate()
    {

    }

    public override void OnSpawn()
    {
        enemies = new List<Enemy>();
        base.OnSpawn();
        beatCD = -1;
        phase = 0;
        UIManager.Instance.PlayerUI.SetBossBarName("Nekomander");
        UIManager.Instance.PlayerUI.ShowBossBar(true);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
        facingRight = transform.position.x < Player.instance.transform.position.x;
    }
    private void ShootBellBullet(Vector2 attackDir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3)(attackDir * 0.5f) + (Vector3.up * 0.5f);
        bullet.direction = attackDir;
        bullet.speed = 12;
        bullet.atk = 5;
        bullet.lifetime = 8;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
                new SpeedOverTimeBehaviour() {start = 0, end = -1, speedPerBeat = 0.25f, targetSpeed = 0 }
            };
        bullet.animator.Play("bellbullet");
        bullet.enemySource = this;
        bullet.OnSpawn();
    }

    public void SpawnRemainBullet(BulletBase originalBullet)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();
        Vector2 areaPos = Vector2.zero; // This should be the arena
        bullet.transform.position = originalBullet.transform.position;

        bullet.direction = (originalBullet.direction + new Vector2(Random.Range(-0.3f, 0.3f), 0)).normalized;
        bullet.speed = 3;
        bullet.atk = 1;
        bullet.lifetime = 4;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
            };
        bullet.OnSpawn();
        bullet.animator.Play("redbullet");
    }

    void MoveTowardsPlayer()
    {
        if (isMoving) return;

        Move();

    }

    public void Move()
    {
        StartCoroutine(MoveToTarget());
    }

    public override bool CanTakeDamage()
    {
        return true;
    }

    IEnumerator MoveToTarget()
    {
        isMoving = true;

        targetPos = Player.instance.transform.position;
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
        velocity = Vector3.zero;
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * 6 * Time.deltaTime);
            //velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return null;
        }
        animator.Play(idleAnimation);
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    public override void Die()
    {
        foreach(Enemy enemy in enemies) enemy.ForceDespawn(enemy);
        enemies.Clear();
        base.Die();
    }
}