using System.Collections;
using UnityEngine;

public class VampiLoli : Enemy
{
    Vector3 dirToPlayer;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;

        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position) + (Vector3)Random.insideUnitCircle * 2;
        Vector2 dir = playerPos - transform.position;
        dir.Normalize();
        dirToPlayer = dir;
        direction = dirToPlayer;
        lifeTime = 20;
        animator.Play("vampiloli_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
        SpawnGroupOfGuards(15, transform.position - ((Vector3)dir * 3));
    }
    protected override void OnBeat()
    {
        if (lifeTime <= 0) return;
        lifeTime--;

        if (lifeTime <= 0)
        {
            ForceDespawn(false);
            return;
        }

        if (Vector2.Distance(transform.position, Player.instance.transform.position) > 15)
        {
            Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
            Vector2 dir = playerPos - transform.position;
            dir.Normalize();
            dirToPlayer = dir;
            direction = dirToPlayer;
            
        }
        if (CanMove())
        {
            MoveTowardsPlayer();
        }
        StartCoroutine(ShootBulletsCoroutine());
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("vampiloli_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.transform.parent = transform;
        bulletSpawnEffect.source = this;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        SpawnBullet(new Vector2(1, 1));
        SpawnBullet(new Vector2(1, -1));
        SpawnBullet(new Vector2(-1, 1));
        SpawnBullet(new Vector2(-1, -1));
        bulletSpawnEffect.Despawn();
    }

    private void SpawnBullet(Vector2 dir)
    {
        DirectionalBullet bullet = PoolManager.Get<DirectionalBullet>();
        bullet.transform.position = transform.position + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 8f;
        bullet.speed = 8f;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 6;
        bullet.transform.localScale = Vector3.one;
        bullet.OnSpawn();
    }

    public void SpawnGroupOfGuards(int number, Vector2 pos)
    {
        float size = 1.5f;
        for (int i = 0; i < number; i++)
        {
            Vector3 random = (Random.insideUnitCircle * size) + pos;
            OjouGuardian e = (OjouGuardian)Enemy.GetEnemyOfType(EnemyType.OjouGuardian);
            e.transform.position = random;
            e.eventMove = dirToPlayer;
            e.OnSpawn();
        }
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
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        isMoving = true;

        float time = 0;

        facingRight = direction.x > 0;
        animator.Play("vampiloli_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = direction * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("vampiloli_normal");
        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return false;
    }
}