using System.Collections;
using UnityEngine;

public class NomSlimeElite : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = 4;
        Sprite.transform.localPosition = Vector3.zero;
        isAttacking = false;
        animator.Play("nomslime_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
    }
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            ShootBullets();
            beatCD = 4;
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
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        SpawnBullet(new Vector2(1, 1));
        SpawnBullet(new Vector2(1, -1));
        SpawnBullet(new Vector2(-1, 1));
        SpawnBullet(new Vector2(-1, -1));

        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir)
    {
        NomEliteBullet bullet = PoolManager.Get<NomEliteBullet>();
        bullet.transform.position = transform.position + (Vector3)(dir * 0.2f) + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 2.5f;
        bullet.speed = 2.5f;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 12;
        bullet.transform.localScale = Vector3.one * 2f;
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
        animator.Play("nomslime_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        Player.TriggerCameraShake(0.5f, 0.2f);
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        animator.Play("nomslime_normal");
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}