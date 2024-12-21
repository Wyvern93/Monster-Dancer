using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungooElite : Enemy
{
    int beatCD;
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

    public void ShootBullets()
    {
        StartCoroutine(ShootBulletsCoroutine());
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("fungoo_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        float playery = Player.instance.GetClosestPlayer(transform.position).y;
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 attackDir = transform.position.y > playery ? Vector2.down : Vector2.up;

        float slice = 360f / 6f;
        for (int i = 0; i < 6; i++)
        {
            Vector2 dir = BulletBase.angleToVector((slice * i) + (Random.Range(-15f, 15f)));
            SpawnBullet(dir);
        }
        
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        bulletSpawnEffect.Despawn();
        isAttacking = false;
        yield break;

    }

    private void SpawnBullet(Vector2 dir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3)(dir * 0.1f) + (Vector3.up * 0.5f);
        bullet.direction = dir;
        bullet.speed = 8;
        bullet.atk = 5;
        bullet.lifetime = 14;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteSpinBehaviour() { start = 0, end = -1, spinSpeed = 100 },
                new SpeedOverTimeBehaviour() { start = 0, end = -1, speedPerBeat = 1f, targetSpeed = 1.5f},
                new PoisonBehaviour(2) { start = 0, end = -1}
            };
        bullet.animator.Play("sporebullet");
        bullet.enemySource = this;
        bullet.OnSpawn();
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