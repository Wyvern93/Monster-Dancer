using System.Collections;
using UnityEngine;

public class FungooElite : Enemy
{
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("fungoo_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
    }
    protected override void OnBeat()
    {
        if (CanMove())
        {
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

        SpawnBullet(Vector2.zero);
        Move();

    }

    private void SpawnBullet(Vector2 dir)
    {
        PoisonBullet bullet = PoolManager.Get<PoisonBullet>();
        bullet.transform.position = transform.position + (Vector3.one * 0.3f);
        bullet.direction = dir;
        bullet.origSpeed = 0f;
        bullet.speed = 0f;
        bullet.enemySource = this;
        bullet.atk = Mathf.Clamp(atk / 8, 1, 1000);
        bullet.lifetime = 8;
        bullet.OnSpawn();
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
        animator.Play("fungoo_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("fungoo_normal");
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