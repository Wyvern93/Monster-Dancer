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
            if ((Vector2)transform.position == Player.position) // If it doesn't, that means no prediction is needed
            {
                Player.instance.TakeDamage(1);
            }
            if (actionList[actionIndex] == EntityAction.MOVE_TOWARDS_PLAYER)
            {
                MoveTowardsPlayer();
            }
            if (actionList[actionIndex] == EntityAction.SPAWN_BULLETS)
            {
                SpawnBullets();
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
    /*
    void MoveTowardsPlayer()
    {
        direction = new Vector2(Player.position.x < transform.position.x ? -1 : Player.position.x > transform.position.x ? 1 : 0, Player.position.y < transform.position.y ? -1 : Player.position.y > transform.position.y ? 1 : 0);
        if (direction.x == -1) facingRight = false;
        if (direction.x == 1) facingRight = true;

        if (!isMoving)
        {
            Move((Vector2)transform.position + direction);
        }
    }*/

    void MoveTowardsPlayer()
    {
        bool horizontalMovement = false;

        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);

        if (Mathf.Abs(transform.position.x - playerPos.x) > Mathf.Abs(transform.position.y - playerPos.y)) horizontalMovement = true;

        direction = Vector2.zero;
        if (horizontalMovement)
        {
            direction.x = Player.position.x < transform.position.x ? -1 : 1;
        }
        else
        {
            direction.y = Player.position.y < transform.position.y ? -1 : 1;
        }

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
        StartCoroutine(SpawnBullet(direction));
    }

    public void Move(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
    }

    private IEnumerator MoveTwice(Vector2 direction)
    {
        Vector2 targetPos = (Vector2)transform.position + direction;
        yield return MoveCoroutine(targetPos);
        targetPos += direction;
        yield return MoveCoroutine(targetPos);
    }

    IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        if (targetPos == Player.position)
        {
            Player.instance.TakeDamage(1);
        }

        isMoving = true;

        SpriteSize = 1.2f;
        Vector3 originalPos = transform.position;
        if (Map.isWallAt(targetPos)) targetPos = transform.position;
        float height = 0;
        float time = 0;
        while (time <= 0.125)
        {
            if (time < 0.0625f)
            {
                height = Mathf.Clamp(height + Time.deltaTime * 4f, 0f, 0.3f);
            }
            else
            {
                height = Mathf.Clamp(height - Time.deltaTime * 4f, 0f, 0.3f);
            }

            transform.position = Vector3.Lerp(originalPos, (Vector3)targetPos, time * 8f);
            time += Time.deltaTime;
            Sprite.transform.localPosition = new Vector3(0, height, 0);
            yield return new WaitForEndOfFrame();
        }
        Sprite.transform.localPosition = Vector3.zero;
        transform.position = targetPos;

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
