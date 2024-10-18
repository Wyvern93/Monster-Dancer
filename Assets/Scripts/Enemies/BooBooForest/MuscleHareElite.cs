using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleHareElite : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        isAttacking = false;
        beatCD = Random.Range(1, 6);
        animator.Play("musclehare_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration();
    }
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

    public void ShootBullets()
    {
        StartCoroutine(ShootBulletsCoroutine());
    }

    private IEnumerator ShootBulletsCoroutine()
    {
        animator.Play("musclehare_normal");
        BulletSpawnEffect bulletSpawnEffect = PoolManager.Get<BulletSpawnEffect>();
        bulletSpawnEffect.source = this;
        bulletSpawnEffect.transform.position = transform.position;
        bulletSpawnEffect.finalScale = 1f;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        float playery = Player.instance.GetClosestPlayer(transform.position).y;
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        Vector2 attackDir = transform.position.y > playery ? Vector2.down : Vector2.up;

        SpawnBullet(Vector2.left, attackDir);
        SpawnBullet(Vector2.right, attackDir);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        SpawnBullet(Vector2.left, attackDir);
        SpawnBullet(Vector2.right, attackDir);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();

        SpawnBullet(Vector2.left, attackDir);
        SpawnBullet(Vector2.right, attackDir);
        AudioController.PlaySound(AudioController.instance.sounds.shootBullet);
        bulletSpawnEffect.Despawn();
        isAttacking = false;
        yield break;

    }

    private void SpawnBullet(Vector2 position, Vector2 attackDir)
    {
        BulletBase bullet = PoolManager.Get<BulletBase>();

        float angleChange = position == Vector2.left ? 120 : -120;

        bullet.transform.position = transform.position + (Vector3)position + (Vector3.up * 0.5f);
        bullet.direction = attackDir;
        bullet.speed = 10;
        bullet.atk = 5;
        bullet.lifetime = 10;
        bullet.transform.localScale = Vector3.one;
        bullet.startOnBeat = true;
        bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 },
                new RotateOverBeatBehaviour() { start = 0, end = -1, rotateAmount = angleChange}
            };
        bullet.animator.Play("punchbullet");
        bullet.enemySource = this;
        bullet.OnSpawn();
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

        float time = 0;
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        Vector2 dir = (playerPos - transform.position).normalized;
        facingRight = dir.x > 0;
        animator.Play("musclehare_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("musclehare_normal");
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