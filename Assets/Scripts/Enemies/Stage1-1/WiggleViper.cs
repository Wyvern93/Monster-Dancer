using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiggleViper : Enemy
{

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
        bulletSpawnEffect.Despawn();
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position;
        bullet.direction = dir;
        bullet.speed = 5;
        bullet.atk = 2;
        bullet.lifetime = 4;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpeedOverTimeBehaviour() { start = 0, end = -1, speedPerBeat = 0.5f, targetSpeed = 7 },
                new PoisonBehaviour(3) { start = 0, end = -1},
            };
        bullet.animator.Play("poisonbullet");
        bullet.OnSpawn();
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
    }

    protected override void Shoot()
    {
        isAttacking = true;
        StartCoroutine(ShootBulletsCoroutine());
    }
}