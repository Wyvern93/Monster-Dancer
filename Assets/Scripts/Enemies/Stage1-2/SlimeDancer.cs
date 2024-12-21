using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEngine;

public class SlimeDancer : Enemy
{
  
    protected override void Shoot()
    {
        isAttacking = true;
        StartCoroutine(ShootBulletsCoroutine());
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("slimedancer_preattack");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        SpawnBullet(dir, 10f, 0f);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        SpawnBullet(dir, 10f, 0f);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        bulletSpawnEffect.Despawn();
        isAttacking = false;
        animator.Play("slimedancer_normal");
        yield break;
        
    }

    private void SpawnBullet(Vector2 dir, float speed, float dist)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3)(dir * dist) + (Vector3.one * 0.5f);
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
        bullet.animator.Play("redbullet");
        bullet.OnSpawn();
    }
}