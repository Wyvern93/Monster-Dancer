using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class ZombieBride : Enemy
{
    protected override void Shoot()
    {
        StartCoroutine(ShootBulletsCoroutine());
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        isAttacking = true;
        // Begin the pre-attack animation.
        animator.Play("zombiebride_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration();

        // Spawn a visual bullet effect.
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;

        // Calculate the DSP time corresponding to the next beat.
        double targetBeatDSP = BeatManager.GetNextBeatDSPTime();

        // Wait until the audio system's DSP time reaches the target.
        yield return new WaitUntil(() => AudioSettings.dspTime >= targetBeatDSP);

        // (Optionally, if you want to wait a fixed beat duration from now instead, you could do:
        // double targetTime = AudioSettings.dspTime + BeatManager.GetBeatDuration();
        // yield return new WaitUntil(() => AudioSettings.dspTime >= targetTime); )

        // If the game is paused or the enemy is stunned, wait until conditions are cleared.
        while (GameManager.isPaused || stunStatus.isStunned())
        {
            yield return null;
        }

        // Calculate direction to the closest player.
        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        float baseAngle = BulletBase.VectorToAngle(dir);

        SpawnBullet(dir, 8);
        for (int j = -1; j < 2; j++)
        {
            float angle = baseAngle + (j * 12f);
            Vector2 bulletDir = BulletBase.angleToVector(angle);
            SpawnBullet(bulletDir, 10);
            SpawnBullet(bulletDir, 12);
        }
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
        bulletSpawnEffect.Despawn();

        // Reset animator speed and play the normal animation.
        animator.speed = (1f / BeatManager.GetBeatDuration());
        animator.Play("zombiebride_move");
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir, float speed)
    {
        //AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3.up * 0.3f);
        bullet.direction = dir;
        bullet.speed = speed;
        bullet.atk = 4;
        bullet.lifetime = 4;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
            };
        bullet.animator.Play("smallredbullet");
        bullet.OnSpawn();
    }
}