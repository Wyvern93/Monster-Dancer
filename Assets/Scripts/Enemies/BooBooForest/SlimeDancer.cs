using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SlimeDancer : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        beatCD = Random.Range(1,10);
        animator.Play("slimedancer_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
        isAttacking = false;
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
        else if(CanMove())
        {
            beatCD--;
            MoveTowardsPlayer();
        }
        
    }

    protected override void OnBehaviourUpdate()
    {

    }

    protected override void OnInitialize()
    {

    }

    public void ShootBullets()
    {
        StartCoroutine(ShootBulletsCoroutine());
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("slimedancer_preattack");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        SpawnBullet(dir, 10f, 0f);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        SpawnBullet(dir, 10f, 0f);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        SpawnBullet(dir, 10f, 0f);

        isAttacking = false;
        animator.Play("slimedancer_normal");
        yield break;
        
    }

    private void SpawnBullet(Vector2 dir, float speed, float dist)
    {
        DirectionalBullet bullet = PoolManager.Get<DirectionalBullet>();
        bullet.transform.position = transform.position + (Vector3)(dir * dist) + (Vector3.one * 0.5f);
        bullet.direction = dir;
        bullet.origSpeed = speed;
        bullet.speed = speed;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 10;
        bullet.OnSpawn();
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