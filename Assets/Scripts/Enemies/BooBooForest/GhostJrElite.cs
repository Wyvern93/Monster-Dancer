using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostJrElite : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = Random.Range(1, 10);
        Sprite.transform.localPosition = Vector3.zero;
        isAttacking = false;
        animator.Play("boojr_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
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

    public void ShootBullets()
    {
        StartCoroutine(ShootBulletsCoroutine());
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("boojr_preattack");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        List<GhostBullet> bullets = new List<GhostBullet>();

        float diff = 360 / 8;
        for (int i = 0; i < 8; i++)
        {
            bullets.Add(SpawnBullet(i * diff, 16f, 1f));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 8f);
        }
        foreach (GhostBullet bullet in bullets)
        {
            bullet.canMove = true;
        }

        isAttacking = false;
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
        animator.Play("boojr_normal");
        yield break;

    }

    private GhostBullet SpawnBullet(float angle, float speed, float dist)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        GhostBullet bullet = PoolManager.Get<GhostBullet>();
        bullet.transform.position = transform.position + (Vector3)(dir * dist) + (Vector3.one * 0.5f);
        bullet.direction = dir;
        bullet.origSpeed = speed;
        bullet.speed = speed;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 10;
        bullet.canMove = false;
        bullet.transform.localScale = Vector3.one * 2; 
        bullet.OnSpawn();
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
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        isMoving = true;
        animator.Play("boojr_move");
        float time = 0;
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("boojr_normal");
        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}