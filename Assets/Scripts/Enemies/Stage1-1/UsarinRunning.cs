using System.Collections;
using UnityEngine;

public class UsarinRunning : Enemy
{
    Vector3 dirToPlayer;
    public override void OnSpawn()
    {
        base.OnSpawn();
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;

        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position) + (Vector3)Random.insideUnitCircle * 2;
        Vector2 dir = playerPos - transform.position;
        dir.Normalize();
        dirToPlayer = dir;
        direction = dirToPlayer;
        lifeTime = 20;
        animator.Play("vampiloli_normal");
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
        SpawnGroupOfFans(15, transform.position - ((Vector3)dir * 3));
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

        if (Vector2.Distance(transform.position, Player.instance.transform.position) > 15)
        {
            Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
            Vector2 dir = playerPos - transform.position;
            dir.Normalize();
            dirToPlayer = dir;
            direction = dirToPlayer;

        }
        if (CanMove())
        {
            MoveTowardsPlayer();
        }
    }


    public void SpawnGroupOfFans(int number, Vector2 pos)
    {
        float size = 1.5f;
        for (int i = 0; i < number; i++)
        {
            Vector3 random = (Random.insideUnitCircle * size) + pos;
            CarrotFan e = (CarrotFan)Enemy.GetEnemyOfType(EnemyType.CarrotFan);
            e.transform.position = random;
            e.eventMove = dirToPlayer;
            e.OnSpawn();
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


    public override bool CanTakeDamage()
    {
        return false;
    }
}