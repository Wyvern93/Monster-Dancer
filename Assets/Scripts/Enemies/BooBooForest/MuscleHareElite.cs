using System.Collections;
using UnityEngine;

public class MuscleHareElite : Enemy
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
        isAttacking = false;
        beatCD = Random.Range(1, 6);
        animator.Play("musclehare_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
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
        animator.Play("musclehare_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        SpawnBullet(Vector2.left, 12f, 0f);
        SpawnBullet(Vector2.right, 12f, 0f);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        SpawnBullet(Vector2.left, 12f, 0f);
        SpawnBullet(Vector2.right, 12f, 0f);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        SpawnBullet(Vector2.left, 12f, 0f);
        SpawnBullet(Vector2.right, 12f, 0f);

        isAttacking = false;
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
        animator.Play("musclehare_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("musclehare_normal");
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