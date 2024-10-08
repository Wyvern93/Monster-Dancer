using System.Collections;
using UnityEngine;

public class WiggleViper : Enemy
{
    int beatCD;
    public bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = 2;
        isAttacking = false;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("wiggleviper_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
    }
    protected override void OnBeat()
    {
        if (isAttacking) return;

        float distance = Vector2.Distance(transform.position, Player.instance.GetClosestPlayer(transform.position));
        if (beatCD <= 0 && distance < 3)
        {
            isAttacking = true;
            ShootBullets();
            beatCD = 2;
        }
        else if (CanMove())
        {
            beatCD--;
            MoveTowardsPlayer();
        }
    }
    private IEnumerator ShootBulletsCoroutine()
    {
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.source = this;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        SpawnBullet(dir);
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir)
    {
        PoisonBullet bullet = PoolManager.Get<PoisonBullet>();
        bullet.transform.position = transform.position + (Vector3)(dir * 0.2f) + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 8f;
        bullet.speed = 8f;
        bullet.enemySource = this;
        bullet.atk = Mathf.Clamp(atk / 8, 1, 1000);
        bullet.lifetime = 8;
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
        animator.Play("wiggleviper_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("wiggleviper_normal");
        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}