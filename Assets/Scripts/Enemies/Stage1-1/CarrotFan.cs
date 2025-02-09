using System.Collections;
using UnityEngine;

public class CarrotFan : Enemy
{
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("carrotfan_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
        lifeTime = 20;
    }
    protected override void OnBeat()
    {
        if (lifeTime <= 0) return;
        lifeTime--;

        if (lifeTime <= 0)
        {
            ForceDespawn(false);
            return;
        }

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

        Move();

    }

    public void Move()
    {
        StartCoroutine(JumpCoroutine());
    }

    protected override IEnumerator JumpCoroutine()
    {
        isMoving = true;

        float time = 0;
        Vector2 dir = eventMove.normalized;

        facingRight = dir.x > 0;

        animator.Play("carrotfan_move");
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return new WaitForEndOfFrame();
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("carrotfan_normal");
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