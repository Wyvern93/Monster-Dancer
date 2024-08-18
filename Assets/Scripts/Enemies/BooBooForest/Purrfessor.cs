using System.Collections;
using UnityEngine;

public class Purrfessor : Enemy
{
    int beatCD;
    bool isAttacking;
    bool horizontal;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = Random.Range(1, 10);
        Sprite.transform.localPosition = Vector3.zero;
        isAttacking = false;
        horizontal = true;
        animator.Play("purrfessor_normal");
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

    void MoveTowardsPlayer()
    {
        if (isMoving) return;

        Move();

    }

    protected override void OnBehaviourUpdate()
    {

    }

    protected override void OnInitialize()
    {

    }

    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("purrfessor_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        if (horizontal)
        {
            SpawnBullet(new Vector2(1, 0));
            SpawnBullet(new Vector2(-1, 0));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration());
            SpawnBullet(new Vector2(1, 0));
            SpawnBullet(new Vector2(-1, 0));
        }
        else
        {
            SpawnBullet(new Vector2(0, 1));
            SpawnBullet(new Vector2(0, -1));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration());
            SpawnBullet(new Vector2(0, 1));
            SpawnBullet(new Vector2(0, -1));
        }
        horizontal = !horizontal;

        //animator.Play("dancearune_normal");
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir)
    {
        StarBullet bullet = PoolManager.Get<StarBullet>();
        bullet.transform.position = transform.position + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 10f;
        bullet.speed = 10f;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one;
        bullet.OnSpawn();
    }

    public void ShootBullets()
    {
        StartCoroutine(ShootBulletsCoroutine());
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

        animator.Play("purrfessor_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("purrfessor_normal");
        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}