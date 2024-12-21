using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurrfessorElite : Enemy
{
    int beatCD;
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            ShootBullets();
            beatCD = 12;
        }
        else if (CanMove())
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
        animator.Play("purrfessor_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        float playery = Player.instance.GetClosestPlayer(transform.position).y;
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 attackDir = transform.position.y > playery ? Vector2.down : Vector2.up;

        float angleBase = 120f;
        for (int i = 0; i < 3; i++)
        {
            SpawnBullet(transform.position, BulletBase.angleToVector(angleBase * i));
        }
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);

        bulletSpawnEffect.Despawn();
        isAttacking = false;
        yield break;

    }

    private void SpawnBullet(Vector2 position, Vector2 attackDir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        float angleChange = position == Vector2.left ? 120 : -120;

        bullet.transform.position = transform.position + (Vector3)(attackDir * 0.5f) + (Vector3.up * 0.5f);
        bullet.direction = attackDir;
        bullet.speed = 9;
        bullet.atk = 5;
        bullet.lifetime = 12;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpawnBulletOnBeatBehaviour(SpawnRemainBullet) { start = 0, end = -1},
                new RotateOverBeatBehaviour() {start = 0, end = -1, rotateAmount = 90f}
            };
        bullet.animator.Play("orbbullet");
        bullet.enemySource = this;
        bullet.OnSpawn();
    }

    public void SpawnRemainBullet(BulletBase originalBullet)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();
        Vector2 areaPos = Vector2.zero; // This should be the arena
        bullet.transform.position = originalBullet.transform.position;

        bullet.direction = (originalBullet.direction + new Vector2(Random.Range(-0.3f, 0.3f), 0)).normalized;
        bullet.speed = 3;
        bullet.atk = 1;
        bullet.lifetime = 4;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1 },
            };
        bullet.OnSpawn();
        bullet.animator.Play("bluestarbullet");
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