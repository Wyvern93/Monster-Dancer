using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythMaiden : Enemy
{
    protected override void Shoot()
    {
        isAttacking = true;
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
        bullet.speed = 6;
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

}