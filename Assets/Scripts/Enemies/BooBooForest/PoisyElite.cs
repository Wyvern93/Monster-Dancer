using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PoisyElite : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = 4;
        isAttacking = false;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("poisy_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
    }
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (beatCD == 0)
        {
            isAttacking = true;
            ShootBullets();
            beatCD = 4;
        }
        if (CanMove() && beatCD < 3)
        {
            MoveTowardsPlayer();
        }
        beatCD--;
    }
    private IEnumerator ShootBulletsCoroutine()
    {
        AudioController.PlaySound(AudioController.instance.sounds.chargeBulletSound);
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.source = this;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
        dir.Normalize();

        SpawnBullet();
        bulletSpawnEffect.Despawn();
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet()
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();
        Vector3 playerPos = Player.instance.transform.position;
        Vector2 dir = (playerPos - transform.position).normalized;

        bullet.transform.position = transform.position + (-Vector3.up * 0.4f);
        bullet.direction = dir;
        bullet.speed = 10;
        bullet.atk = 5;
        bullet.lifetime = 8;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new PoisonBehaviour(3) { start = 0, end = -1},
                new HomingToPlayerBehaviour(Player.instance.gameObject, 0.02f) {start = 0, end = -1 }
            };
        bullet.animator.Play("bigpoisonbullet");
        bullet.OnSpawn();
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
    }

    public void ShootBullets()
    {
        StartCoroutine(ShootBulletsCoroutine());
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
        animator.Play("poisy_move");
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
        animator.Play("poisy_normal");
        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}