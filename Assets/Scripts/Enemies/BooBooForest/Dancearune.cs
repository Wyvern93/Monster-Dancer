using System.Collections;
using UnityEngine;

public class Dancearune : Enemy
{
    int beatCD;
    bool isAttacking;
    bool diagonal;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = Random.Range(1,10);
        Sprite.transform.localPosition = Vector3.zero;
        isAttacking = false;
        diagonal = true;
        animator.Play("dancearune_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
    }
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            ShootBullets();
            beatCD = 10;
        }
        else if (CanMove())
        {
            beatCD--;
            MoveTowardsPlayer();
        }
    }
    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("dancearune_preattack");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        if (diagonal)
        {
            SpawnBullet(new Vector2(1, 1));
            SpawnBullet(new Vector2(1, -1));
            SpawnBullet(new Vector2(-1, 1));
            SpawnBullet(new Vector2(-1, -1));
        }
        else
        {
            SpawnBullet(new Vector2(0, 1));
            SpawnBullet(new Vector2(0, -1));
            SpawnBullet(new Vector2(-1, 0));
            SpawnBullet(new Vector2(1, 0));
        }
        diagonal = !diagonal;
        bulletSpawnEffect.Despawn();
        animator.Play("dancearune_normal");
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir)
    {
        PetalBullet bullet = PoolManager.Get<PetalBullet>();
        bullet.transform.position = transform.position + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 4f;
        bullet.speed = 4f;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one;
        bullet.follow = false;
        bullet.OnSpawn();
    }

    public void ShootBullets()
    {
        StartCoroutine(ShootBulletsCoroutine());
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
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        while (time <= BeatManager.GetBeatDuration() / 3f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = dir * speed * 8;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
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