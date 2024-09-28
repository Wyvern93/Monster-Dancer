using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        
        for (int i = 0; i < 4; i++)
        {
            Vector2 playerdir = Player.instance.GetClosestPlayer(transform.position + (-Vector3.up * 0.4f)) - transform.position;
            playerdir.Normalize();

            float baseAngle = BulletBase.VectorToAngle(playerdir);
            for (int j = -1; j < 2; j++)
            {
                float angle = baseAngle + (j * 15f);
                Vector2 dir = BulletBase.angleToVector(angle);
                SpawnBullet(dir);
            }
            
            float time = BeatManager.GetBeatDuration();
            while (time > 0)
            {
                while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        }

        bulletSpawnEffect.Despawn();
        isAttacking = false;
        yield break;
    }

    private void SpawnBullet(Vector2 dir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        bullet.transform.position = transform.position;
        bullet.direction = dir;
        bullet.speed = 10;
        bullet.atk = 5;
        bullet.lifetime = 8;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.enemySource = this;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new SpeedOverTimeBehaviour() { start = 0, end = -1, speedPerBeat = 0.5f, targetSpeed = 7 },
                new PoisonBehaviour(3) { start = 0, end = -1},
            };
        bullet.animator.Play("poisonbullet");
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