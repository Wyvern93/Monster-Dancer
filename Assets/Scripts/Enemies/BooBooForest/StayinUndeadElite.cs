using System.Collections;
using UnityEngine;

public class StayinUndeadElite : Enemy
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
        beatCD = 2;
        AItype = 0;
        isAttacking = false;
        animator.Play("stayinundead_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
    }
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            StartCoroutine(ShootBullets());
            beatCD = 4;
        }
        else if (CanMove())
        {
            beatCD--;
            MoveTowardsPlayer();
        }
    }

    IEnumerator ShootBullets()
    {
        isAttacking = true;
        animator.Play("stayinundead_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        if (AItype == 0)
        {
            SpawnDirectionalBullet(new Vector2(-1f, -1f));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration());
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            SpawnDirectionalBullet(new Vector2(-1f, 0f));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration());
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            SpawnDirectionalBullet(new Vector2(-1f, 1f));
        }
        else if (AItype == 1)
        {
            SpawnDirectionalBullet(new Vector2(1f, -1f));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration());
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            SpawnDirectionalBullet(new Vector2(1f, 0f));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration());
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            SpawnDirectionalBullet(new Vector2(1f, 1f));
        }
        else
        {
            SpawnSpiralBullet(0);
            SpawnSpiralBullet(120);
            SpawnSpiralBullet(240);
        }
        bulletSpawnEffect.Despawn();
        if (AItype >= 2) AItype = 0;
        else AItype++;
        //animator.Play("dancearune_normal");
        isAttacking = false;
    }

    private void SpawnDirectionalBullet(Vector2 dir)
    {
        DirectionalBullet bullet = PoolManager.Get<DirectionalBullet>();
        bullet.transform.position = transform.position + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 15f;
        bullet.speed = 15f;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one;
        bullet.OnSpawn();
    }

    private void SpawnSpiralBullet(float angle)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        SpiralBullet bullet = PoolManager.Get<SpiralBullet>();
        bullet.transform.position = transform.position + (Vector3)(dir * 0.4f) + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 2.5f;
        bullet.speed = 2.5f;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 6;
        bullet.orbitAngle = angle;
        bullet.transform.localScale = Vector3.one * 2f;
        bullet.orbitSpeed = 2f;
        bullet.directionalVelocity = Vector3.zero;
        bullet.OnSpawn();
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
        animator.Play("stayinundead_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("stayinundead_normal");
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