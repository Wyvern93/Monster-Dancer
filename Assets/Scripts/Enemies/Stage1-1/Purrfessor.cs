using System.Collections;
using System.Collections.Generic;
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
            if (isCloseEnoughToShoot())
            {
                isAttacking = true;
                ShootBullets();
            }
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
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        if (horizontal)
        {
            SpawnBullet(new Vector2(1, 0));
            SpawnBullet(new Vector2(-1, 0));
            AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
            yield return new WaitForSeconds(BeatManager.GetBeatDuration());
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            SpawnBullet(new Vector2(1, 0));
            SpawnBullet(new Vector2(-1, 0));
            AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        }
        else
        {
            SpawnBullet(new Vector2(0, 1));
            SpawnBullet(new Vector2(0, -1));
            AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
            yield return new WaitForSeconds(BeatManager.GetBeatDuration());
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            SpawnBullet(new Vector2(0, 1));
            SpawnBullet(new Vector2(0, -1));
            AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        }
        bulletSpawnEffect.Despawn();
        horizontal = !horizontal;

        //animator.Play("dancearune_normal");
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3.up * 0.3f);
        bullet.direction = dir;
        bullet.speed = 10;
        bullet.atk = 5;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 }
            };
        bullet.animator.Play("starbullet");
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
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
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