using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StayinUndeadElite : Enemy
{
    int beatCD;
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            StartCoroutine(ShootBullets());
            beatCD = 4;
        }
        else if (CanMove())
        {
            beatCD--;
            MoveTowardsPlayer();
        }
    }

    IEnumerator ShootBullets()
    {
        isAttacking = true;
        animator.Play("stayinundead_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 baseDir = Vector2.left;
        float shootAngle = BulletBase.VectorToAngle(baseDir);
        
        SpawnDirectionalBullet(shootAngle - 10);
        SpawnDirectionalBullet(shootAngle);
        SpawnDirectionalBullet(shootAngle + 10);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        baseDir = Vector2.right;
        shootAngle = BulletBase.VectorToAngle(baseDir);

        SpawnDirectionalBullet(shootAngle - 10);
        SpawnDirectionalBullet(shootAngle);
        SpawnDirectionalBullet(shootAngle + 10);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        baseDir = Vector2.up;
        shootAngle = BulletBase.VectorToAngle(baseDir);

        SpawnDirectionalBullet(shootAngle - 10);
        SpawnDirectionalBullet(shootAngle);
        SpawnDirectionalBullet(shootAngle + 10);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        baseDir = Vector2.down;
        shootAngle = BulletBase.VectorToAngle(baseDir);

        SpawnDirectionalBullet(shootAngle - 10);
        SpawnDirectionalBullet(shootAngle);
        SpawnDirectionalBullet(shootAngle + 10);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        ShootSpiral();

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        bulletSpawnEffect.Despawn();

        //animator.Play("dancearune_normal");
        isAttacking = false;
    }

    private void SpawnDirectionalBullet(float angle)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        Vector2 dir = BulletBase.angleToVector(angle);
        bullet.transform.position = transform.position + (Vector3)(dir * 0.2f) + (Vector3.up * 0.5f);
        bullet.direction = dir;
        bullet.speed = 9;
        bullet.atk = 5;
        bullet.lifetime = 8;
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

    private void SpawnMusicBullet(float angle)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        Vector2 dir = BulletBase.angleToVector(angle);
        bullet.transform.position = transform.position + (Vector3)(dir * 0.2f) + (Vector3.up * 0.5f);
        bullet.direction = dir;
        bullet.speed = 7;
        bullet.atk = 5;
        bullet.lifetime = 7;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteWaveBehaviour() { start = 0, end = -1 },
                new RotateOverBeatBehaviour() { start = 0, end = -1, rotateAmount = 90 }
            };
        bullet.animator.Play("notebullet");
        bullet.OnSpawn();
    }

    private void ShootSpiral()
    {
        float anglebase = 360f / 12f;
        for (int i = 0; i < 12; i++)
        {
            SpawnMusicBullet(i * anglebase);
        }
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
    }

    private void SpawnSpiralBullet(float angle)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        SpiralBullet bullet = PoolManager.Get<SpiralBullet>();
        bullet.transform.position = transform.position + (Vector3)(dir * 0.4f) + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 2.5f;
        bullet.speed = 2.5f;
        bullet.enemySource = this;
        bullet.atk = atk / 4;
        bullet.lifetime = 6;
        bullet.orbitAngle = angle;
        bullet.transform.localScale = Vector3.one * 2f;
        bullet.orbitSpeed = 2f;
        bullet.directionalVelocity = Vector3.zero;
        bullet.OnSpawn();
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

    public void Move()
    {
        StartCoroutine(MoveCoroutine());
    }
    public override bool CanTakeDamage()
    {
        return true;
    }
}