using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostJr : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = Random.Range(1, 16);
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
            if (isCloseEnoughToShoot())
            {
                isAttacking = true;
                ShootBullets();
                AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
            }
            beatCD = 16;
        }
        if (CanMove())
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
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        SpawnBullet(dir, 16f, 0f);
        bulletSpawnEffect.Despawn();
        isAttacking = false;
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
        animator.Play("boojr_normal");
        yield break;

    }

    private void SpawnBullet(Vector2 dir, float speed, float dist)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3.up * 0.3f);
        bullet.direction = dir;
        bullet.speed = 3;
        bullet.atk = 5;
        bullet.lifetime = 12;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
            };
        bullet.animator.Play("ghostbullet");
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
        animator.Play("boojr_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("boojr_normal");
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