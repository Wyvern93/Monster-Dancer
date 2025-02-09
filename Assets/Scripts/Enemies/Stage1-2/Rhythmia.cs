using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhythmia : Enemy
{
    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("rhythmia_preattack");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        
        SpawnBullet(0);
        SpawnBullet(120);
        SpawnBullet(240);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        bulletSpawnEffect.Despawn();
        animator.Play("rhythmia_normal");
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(float angle)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        Vector2 dir = BulletBase.angleToVector(angle);
        bullet.transform.position = transform.position + (Vector3)(dir * 0.2f) + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.speed = 8f;
        bullet.atk = 5;
        bullet.lifetime = 3;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteWaveBehaviour() { start = 0, end = -1 },
                new RotateOverBeatBehaviour() { start = 0, end = -1, rotateAmount = 460f}
            };
        bullet.animator.Play("notebullet");
        bullet.OnSpawn();
    }

    protected override void Shoot()
    {
        isAttacking = true;
        StartCoroutine(ShootBulletsCoroutine());
    }

}