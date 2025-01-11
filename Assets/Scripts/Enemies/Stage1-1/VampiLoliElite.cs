using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VampiLoliElite : Enemy
{
    Vector3 dirToPlayer;
    int beats;
    private Vector3 targetPos;
    public override void OnSpawn()
    {
        base.OnSpawn();
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position) + (Vector3)Random.insideUnitCircle * 2;
        Vector2 dir = playerPos - transform.position;
        dir.Normalize();
        beats = 0;
        dirToPlayer = dir;
        direction = dirToPlayer;
    }

    public void SpawnGuards()
    {
        float angle, x, y;
        angle = Random.Range(0, 360f);
        x = Player.instance.transform.position.x + (15 * Mathf.Cos(angle));
        y = Player.instance.transform.position.y + (15 * Mathf.Sin(angle));

        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

        Vector3 playerPos = Player.instance.GetClosestPlayer(spawnPos) + (Vector3)Random.insideUnitCircle * 2;
        Vector2 dir = playerPos - spawnPos;
        dir.Normalize();

        float size = 1.5f;
        for (int i = 0; i < 10; i++)
        {
            Vector3 random = (Vector3)(Random.insideUnitCircle * size) + spawnPos;
            OjouGuardian e = PoolManager.Get<OjouGuardian>();
            e.transform.position = random;
            e.eventMove = dir;
            e.OnSpawn();
        }
    }

    protected override void OnBeat()
    {
        if (beats > 0) beats--;
        if (beats <= 0)
        {
            beats = 12;
            StartCoroutine(ShootBulletsCoroutine());
            SpawnGuards();
        }

        if (Vector2.Distance(transform.position, Player.instance.transform.position) > 15)
        {
            Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
            Vector2 dir = playerPos - transform.position;
            dir.Normalize();
            dirToPlayer = dir;
            direction = dirToPlayer;
            
        }
        if (CanMove())
        {
            MoveTowardsPlayer();
        }
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("vampiloli_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.transform.parent = transform;
        bulletSpawnEffect.source = this;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        SpawnBullet(new Vector2(1, 1));
        SpawnBullet(new Vector2(1, -1));
        SpawnBullet(new Vector2(-1, 1));
        SpawnBullet(new Vector2(-1, -1));
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        bulletSpawnEffect.Despawn();
    }

    private void SpawnBullet(Vector2 dir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.speed = 8;
        bullet.atk = 3;
        bullet.lifetime = 12;
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

    IEnumerator MoveToTarget()
    {
        isMoving = true;

        float time = 0;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            velocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * 6);
            if (transform.position == targetPos) break;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play(idleAnimation);
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);

        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    protected override IEnumerator MoveCoroutine()
    {
        isMoving = true;

        float time = 0;
        targetPos = Player.instance.transform.position;
        Vector2 dir = (targetPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play(idleAnimation);
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}