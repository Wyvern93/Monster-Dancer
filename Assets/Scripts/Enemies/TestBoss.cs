using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : Boss
{
    private List<EntityAction> actionList = new List<EntityAction>()
    {
        EntityAction.MOVE_TOWARDS_PLAYER,
        EntityAction.MOVE_TOWARDS_PLAYER,
        EntityAction.MOVE_TOWARDS_PLAYER,
        EntityAction.MOVE_TOWARDS_PLAYER,
        EntityAction.WAIT,
        EntityAction.JUMP
    };

    private bool isJumping;

    private int actionIndex = 0;
    [SerializeField] BoxCollider2D boxCollider;
    public override void OnSpawn()
    {
        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
    }

    protected override void OnBeat()
    {
        Debug.Log("isGameBeat");
        if (CanMove())
        {
            if (isJumping) return;

            if ((Vector2)transform.position == Player.position) // If it doesn't, that means no prediction is needed
            {
                Player.instance.TakeDamage(5);
            }

            if (actionList[actionIndex] == EntityAction.MOVE_TOWARDS_PLAYER)
            {
                MoveTowardsPlayer();
            }
            if (actionList[actionIndex] == EntityAction.SPAWN_BULLETS)
            {
                SpawnBullets();
            }

            if (actionList[actionIndex] == EntityAction.JUMP)
            {
                StartCoroutine(JumpToPlayer());
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
        if (Mathf.Abs(transform.position.x - Player.position.x) > Mathf.Abs(transform.position.y - Player.position.y)) horizontalMovement = true;

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
        direction = new Vector2(Player.position.x < transform.position.x ? -1 : Player.position.x > transform.position.x ? 1 : 0, Player.position.y < transform.position.y ? -1 : Player.position.y > transform.position.y ? 1 : 0);
        StartCoroutine(SpawnBullet(direction));
    }

    public void Move(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
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
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        Player.TriggerCameraShake(0.2f, 0.2f);
        Sprite.transform.localPosition = Vector3.zero;
        transform.position = targetPos;

        isMoving = false;
        yield break;
    }

    private IEnumerator SpawnBullet(Vector2 direction)
    {
        // Sync with next beat
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();

        BulletSpawnEffect spawnEffect = PoolManager.Get<BulletSpawnEffect>();
        spawnEffect.transform.position = new Vector3(transform.position.x + direction.x, transform.position.y + direction.y);
        yield return new WaitForEndOfFrame();

        // The spawnEffect lasts a beat long
        while (spawnEffect.gameObject.activeSelf) yield return new WaitForEndOfFrame();

        Bullet bullet = PoolManager.Get<Bullet>();
        bullet.transform.position = new Vector3(spawnEffect.transform.position.x, spawnEffect.transform.position.y);
        bullet.direction = direction * 2f;
        bullet.OnSpawn();
        yield break;
    }

    private IEnumerator JumpToPlayer()
    {
        isJumping = true;
        boxCollider.enabled = false;
        while (Sprite.transform.localPosition.y < 12)
        {
            Sprite.transform.localPosition = Vector3.MoveTowards(Sprite.transform.localPosition, new Vector3(Sprite.transform.localPosition.x, 12, Sprite.transform.localPosition.z), Time.deltaTime * 32f);
            yield return new WaitForEndOfFrame();
        }
        Vector2 position = Player.position;
        transform.position = position;

        yield return new WaitForEndOfFrame();
        while (!BeatManager.isGameBeat)
        {
            yield return null;
        }
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);
        StartCoroutine(SpawnBullet(new Vector2(-1, -1)));
        StartCoroutine(SpawnBullet(new Vector2(-1, 1)));
        StartCoroutine(SpawnBullet(new Vector2(-1, 0)));
        StartCoroutine(SpawnBullet(new Vector2(0, -1)));
        StartCoroutine(SpawnBullet(new Vector2(0, 1)));
        StartCoroutine(SpawnBullet(new Vector2(1, -1)));
        StartCoroutine(SpawnBullet(new Vector2(1, 1)));
        StartCoroutine(SpawnBullet(new Vector2(1, 0)));

        Debug.Log("Wait for 1 beat");
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());
        Debug.Log("Start falling at" + Time.time);
        while (Sprite.transform.localPosition.y > 0)
        {
            Sprite.transform.localPosition = Vector3.MoveTowards(Sprite.transform.localPosition, Vector3.zero, Time.deltaTime * 32f);
            yield return new WaitForEndOfFrame();
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        Player.TriggerCameraShake(0.4f, 0.2f);
        boxCollider.enabled = true;
        isJumping = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
    }
}
