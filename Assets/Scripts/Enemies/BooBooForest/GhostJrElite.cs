using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostJrElite : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = Random.Range(1, 10);
        Sprite.transform.localPosition = Vector3.zero;
        isAttacking = false;
        animator.Play("boojr_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
    }
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            ShootBullets();
            beatCD = 10;
        }
        else if (CanMove())
        {
            beatCD--;
            MoveTowardsPlayer();
        }

    }

    public void ShootBullets()
    {
        StartCoroutine(ShootBulletsCoroutine());
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        animator.Play("boojr_preattack");
        animator.speed = 1f / BeatManager.GetBeatDuration();
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        List<BulletBase> tempbullets = new List<BulletBase>();
        float diff = 360f / 12f;
        for (int i = 0; i < 12; i++)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

            tempbullets.Add(SpawnBullet(i * diff, 10 + (i % 3), 1 + ((i % 3) / 2f)));
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 12f);
        }
        foreach (BulletBase bullet in tempbullets)
        {
            bullet.beat = 1;
        }
        float time = BeatManager.GetBeatDuration();
        while (time > 0)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        bulletSpawnEffect.Despawn();
        isAttacking = false;
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
        animator.Play("boojr_normal");
        yield break;

    }

    private BulletBase SpawnBullet(float angle, float speed, float dist)
    {
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position + ((Vector3)dir * -dist) + (Vector3.up * 0.5f);// + (Vector3)(dir * -dist) + (Vector3.one * 0.5f);
        bullet.direction = dir;
        bullet.speed = 0;
        bullet.atk = 5;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one * 2f;
        bullet.startOnBeat = false;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpeedOverTimeBehaviour() { start = 2, end = -1, speedPerBeat = 3f, targetSpeed = speed},
                new ZigZagBehaviour() { start = 0, end= -1, triggerOnce = false }
            };
        bullet.animator.Play("ghostbullet");
        bullet.OnSpawn();
        bullet.beat = 0;
        bullet.enemySource = this;
        return bullet;
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

    IEnumerator MoveCoroutine()
    {
        isMoving = true;
        animator.Play("boojr_move");
        float time = 0;
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("boojr_normal");
        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}