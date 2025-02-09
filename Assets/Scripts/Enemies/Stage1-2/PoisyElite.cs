using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisyElite : Enemy
{
    int beatCD;
    private Vector3 targetPos;
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

        
        for (int i = 0; i < 3; i++)
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
        bullet.speed = 9;
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

    public override void OnSpawn()
    {
        base.OnSpawn();
        UIManager.Instance.PlayerUI.SetBossBarName("Deadly Poisy");
        UIManager.Instance.PlayerUI.ShowBossBar(true);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
    }

    void MoveTowardsPlayer()
    {
        if (isMoving) return;

        Move();

    }

    public void Move()
    {
        StartCoroutine(JumpCoroutine());
    }

    public override bool CanTakeDamage()
    {
        return true;
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

    protected override IEnumerator JumpCoroutine()
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
}