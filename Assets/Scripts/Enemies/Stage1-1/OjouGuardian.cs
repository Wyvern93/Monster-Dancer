using System.Collections;
using UnityEngine;

public class OjouGuardian : Enemy
{
    public override void OnSpawn()
    {
        base.OnSpawn();
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

    public override bool CanTakeDamage()
    {
        return true;
    }
}