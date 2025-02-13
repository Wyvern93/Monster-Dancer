using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dancearune : Enemy
{
    bool diagonal;
    public override void OnSpawn()
    {
        base.OnSpawn();
        diagonal = true;
    }
    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("dancearune_preattack");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

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

        bullet.transform.position = transform.position + (Vector3.up * 0.3f);
        bullet.direction = dir;
        bullet.speed = 4;
        bullet.atk = 5;
        bullet.lifetime = 8;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 }
            };
        bullet.animator.Play("petalbullet");
        bullet.OnSpawn();
    }

    protected override void Shoot()
    {
        isAttacking = true;
        StartCoroutine(ShootBulletsCoroutine());
    }
}