using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancearuneElite : Enemy
{
    int beatCD;
    bool diagonal;
    public override void OnSpawn()
    {
        base.OnSpawn();
        diagonal = true;
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
    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("dancearune_preattack");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        if (diagonal)
        {
            SpawnBullet(new Vector2(1, 1));
            SpawnBullet(new Vector2(1, -1));
            SpawnBullet(new Vector2(-1, 1));
            SpawnBullet(new Vector2(-1, -1));
        }
        else
        {
            SpawnBullet(new Vector2(0, 1));
            SpawnBullet(new Vector2(0, -1));
            SpawnBullet(new Vector2(-1, 0));
            SpawnBullet(new Vector2(1, 0));
        }

        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);

        diagonal = !diagonal;
        bulletSpawnEffect.Despawn();
        animator.Play("dancearune_normal");
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + ((Vector3)dir * 0.3f) + (Vector3.up * 0.5f);
        bullet.direction = dir;
        bullet.speed = 8;
        bullet.atk = 5;
        bullet.lifetime = 14;
        bullet.transform.localScale = Vector3.one * 2f;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpeedOverTimeBehaviour() { start = 0, end = 4, speedPerBeat = 0.6f, targetSpeed = 0},
                new SpeedOverTimeBehaviour() { start = 4, end = -1, speedPerBeat = 10, targetSpeed = 14 },
                new HomingToPlayerBehaviour(Player.instance.gameObject, 10) { start = 4, end = 4 }
            };
        bullet.animator.Play("petalbullet");
        bullet.enemySource = this;
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

    public override bool CanTakeDamage()
    {
        return true;
    }
}