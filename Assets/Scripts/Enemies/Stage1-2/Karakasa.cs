using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Karakasa : Enemy
{

    protected override IEnumerator JumpCoroutine()
    {
        SpawnBullet(0);
        SpawnBullet(120);
        SpawnBullet(240);
        isMoving = true;
        animator.Play("karakasa_move");
        float time = 0;
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return null;
        }
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("karakasa_normal");
        isMoving = false;
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
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new RotateOverBeatBehaviour() { start = 0, end = -1, rotateAmount = 460f}
            };
        bullet.animator.Play("redbullet");
        bullet.OnSpawn();
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}