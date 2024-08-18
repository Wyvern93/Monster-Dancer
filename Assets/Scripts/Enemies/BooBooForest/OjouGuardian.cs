using System.Collections;
using UnityEngine;

public class OjouGuardian : Enemy
{
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("ojouguard_normal");
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
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        isMoving = true;

        float time = 0;
        Vector2 dir = eventMove.normalized;

        animator.Play("ojouguard_move");
        facingRight = dir.x > 0;
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("ojouguard_normal");
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