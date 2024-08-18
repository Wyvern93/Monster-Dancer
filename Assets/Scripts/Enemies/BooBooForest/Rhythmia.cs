using System.Collections;
using UnityEngine;

public class Rhythmia : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = Random.Range(1,6);
        Sprite.transform.localPosition = Vector3.zero;
        isAttacking = false;
        animator.Play("rhythmia_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
    }
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            ShootBullets();
            beatCD = 6;
        }
        else if (CanMove())
        {
            beatCD--;
            MoveTowardsPlayer();
        }
    }
    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("rhythmia_preattack");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        
        SpawnBullet(0);
        SpawnBullet(120);
        SpawnBullet(240);
        
        animator.Play("rhythmia_normal");
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(float angle)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        SpiralBullet bullet = PoolManager.Get<SpiralBullet>();
        bullet.transform.position = transform.position + (Vector3)(dir * 0.2f) + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 2.5f;
        bullet.speed = 2.5f;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 3;
        bullet.orbitAngle = angle;
        bullet.transform.localScale = Vector3.one * 2f;
        bullet.orbitSpeed = 2f;
        bullet.directionalVelocity = Vector3.zero;
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