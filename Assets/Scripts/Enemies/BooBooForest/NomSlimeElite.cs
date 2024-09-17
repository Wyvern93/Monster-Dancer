using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomSlimeElite : Enemy
{
    int beatCD;
    bool isAttacking;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        beatCD = 0;
        Sprite.transform.localPosition = Vector3.zero;
        isAttacking = false;
        animator.Play("nomslime_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
    }
    protected override void OnBeat()
    {
        if (isAttacking) return;
        if (isMoving) return;

        if (beatCD == 0) MoveTowardsPlayer();
        if (beatCD >= 4) beatCD = -1;
        beatCD++;
    }

    private void SpawnBullets()
    {
        for (int i = 0; i < 12; i++)
        {
            BulletBase bullet = PoolManager.Get<BulletBase>();

            float angle = (360f / 12f) * i * Mathf.Deg2Rad;
            Vector2 localDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));

            bullet.transform.position = transform.position + (-Vector3.up * 0.4f);
            
            bullet.direction = localDir;
            bullet.speed = 6;
            bullet.atk = 4;
            bullet.lifetime = 6;
            bullet.transform.localScale = Vector3.one;
            bullet.startOnBeat = true;
            bullet.behaviours = new List<BulletBehaviour>
            {
                new SpriteLookAngleBehaviour() { start = 0, end = -1 }
            };
            bullet.animator.Play("redbullet");
            bullet.OnSpawn();
        }
        AudioController.PlaySound(AudioController.instance.sounds.bulletwaveShootSound);
    }

    protected override void OnBehaviourUpdate()
    {

    }

    protected override void OnInitialize()
    {

    }

    void MoveTowardsPlayer()
    {
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
        animator.Play("nomslime_elite_move");
        while (time <= BeatManager.GetBeatDuration() * 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        Player.TriggerCameraShake(1f, 0.3f);
        SpawnBullets();
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        animator.Play("nomslime_normal");
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}