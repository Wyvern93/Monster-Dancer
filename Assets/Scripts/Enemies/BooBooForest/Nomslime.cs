using System.Collections;
using UnityEngine;

public class NomSlime : Enemy
{
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play("nomslime_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
    }
    protected override void OnBeat()
    {
        if (!CanMove()) return;

        if (AItype == 0)
        {
            MoveTowardsPlayer();
        }
        else if (AItype == 2)
        {
            lifeTime--;
            if (lifeTime <= 0)
            {
                ForceDespawn();
                return;
            }
            else
            {
                DashTowardsPlayer();
            }
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

    void DashTowardsPlayer()
    {
        if (isMoving) return;

        StartCoroutine(DashCoroutine());
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
        animator.Play("nomslime_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            velocity = dir * speed * 6;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        animator.Play("nomslime_normal");
        yield break;
    }

    IEnumerator DashCoroutine()
    {
        isMoving = true;

        float time = 0;
        facingRight = eventMove.x > 0;
        animator.Play("nomslime_move");
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            velocity = eventMove * 10f;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
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