using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{

    private List<EntityAction> actionList = new List<EntityAction>()
    {
        EntityAction.WAIT,
        EntityAction.SPAWN_BULLETS,
        EntityAction.SPAWN_BULLETS,
        EntityAction.WAIT,
        EntityAction.MOVE_TOWARDS_PLAYER,
        EntityAction.MOVE_TOWARDS_PLAYER,
        EntityAction.MOVE_TOWARDS_PLAYER,
    };

    private int actionIndex = 0;

    public override void OnSpawn()
    {
        Map.Instance.enemiesAlive.Add(this);
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
    }
    protected override void OnBeat()
    {
        if (CanMove())
        {
            if (actionList[actionIndex] == EntityAction.SPAWN_BULLETS)
            {
                if (Vector2.Distance(transform.position, Player.instance.transform.position) > 10) MoveTowardsPlayer();
                else SpawnBullets();
            }
            if (actionList[actionIndex] == EntityAction.MOVE_TOWARDS_PLAYER)
            {
                MoveTowardsPlayer();
            }
            if (actionList[actionIndex] == EntityAction.WAIT)
            {
                if (Vector2.Distance(transform.position, Player.instance.transform.position) > 10) MoveTowardsPlayer();
            }
            actionIndex++;
            if (actionIndex == actionList.Count) actionIndex = 0;
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
        bool horizontalMovement = false;

        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);

        if (Mathf.Abs(transform.position.x - playerPos.x) > Mathf.Abs(transform.position.y - playerPos.y)) horizontalMovement = true;

        direction = Vector2.zero;

        if (direction.x == -1) facingRight = false;
        if (direction.x == 1) facingRight = true;

        if (!isMoving)
        {
            Move((Vector2)transform.position + direction);
        }
    }

    void SpawnBullets()
    {
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        direction = new Vector2(playerPos.x < transform.position.x ? -1 : playerPos.x > transform.position.x ? 1 : 0, playerPos.y < transform.position.y ? -1 : playerPos.y > transform.position.y ? 1 : 0);
        Vector2 dir = (playerPos - transform.position).normalized;
        StartCoroutine(SpawnBullet(dir));
    }

    public void Move(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
    }

    IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        isMoving = true;

        SpriteSize = 1.2f;
        Vector3 originalPos = transform.position;
        float time = 0;
        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
        Vector2 dir = (playerPos - transform.position).normalized;
        float speed = 0.8f;
        while (time <= BeatManager.GetBeatDuration() / 3f)
        {
            velocity = dir * speed * 8;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }

    private IEnumerator SpawnBullet(Vector2 direction)
    {
        // First beat warning
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();
        BulletSpawnEffect spawnEffect = PoolManager.Get<BulletSpawnEffect>();
        spawnEffect.transform.position = new Vector3(transform.position.x + direction.x, transform.position.y + direction.y);
        yield return new WaitForEndOfFrame();

        // Second beat finish
        while (spawnEffect.gameObject.activeSelf) yield return new WaitForEndOfFrame();

        // Spawn
        Bullet bullet = PoolManager.Get<Bullet>();
        bullet.transform.position = new Vector3(spawnEffect.transform.position.x, spawnEffect.transform.position.y);
        bullet.direction = direction;
        bullet.enemySource = this;
        bullet.OnSpawn();
        //bullet.beatsLeft = 30;
        yield break;
    }
}
