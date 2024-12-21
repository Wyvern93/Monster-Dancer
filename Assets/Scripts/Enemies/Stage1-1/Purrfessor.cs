using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purrfessor : Enemy
{
    bool horizontal;
    public override void OnSpawn()
    {
        base.OnSpawn();
        horizontal = true;
    }

    protected override void Shoot()
    {
        isAttacking = true;
        StartCoroutine(ShootBulletsCoroutine());
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
}