using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RhythMaiden : Enemy
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
        animator.Play("rhythmaiden_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        isAttacking = false;
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
        animator.Play("rhythmaiden_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 playerdir = Player.instance.GetClosestPlayer(transform.position + (-Vector3.up * 0.4f)) - transform.position;
        playerdir.Normalize();

        float baseAngle = BulletBase.VectorToAngle(playerdir);
        for (int j = -1; j < 2; j++)
        {
            float angle = baseAngle + (j * 10f);
            Vector2 dir = BulletBase.angleToVector(angle);
            SpawnBullet(dir);
        }

        float time = BeatManager.GetBeatDuration();
        while (time > 0)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        bulletSpawnEffect.Despawn();
        isAttacking = false;
        animator.Play("rhythmaiden_normal");
        yield break;
        
    }

    private void SpawnBullet(Vector2 dir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3)(dir * 0.5f) + (Vector3.one * 0.5f);
        bullet.direction = dir;
        bullet.speed = 4;
        bullet.atk = 3;
        bullet.lifetime = 6;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 }
            };
        bullet.animator.Play("daggerbullet");
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
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
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