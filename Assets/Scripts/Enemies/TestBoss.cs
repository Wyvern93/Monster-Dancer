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
        Stage.Instance.enemiesAlive.Add(this);
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
        if (Mathf.Abs(transform.position.x - Player.instance.transform.position.x) > Mathf.Abs(transform.position.y - Player.instance.transform.position.y)) horizontalMovement = true;

        direction = Vector2.zero;
        if (horizontalMovement)
        {
            direction.x = Player.instance.transform.position.x < transform.position.x ? -1 : 1;
        }
        else
        {
            direction.y = Player.instance.transform.position.y < transform.position.y ? -1 : 1;
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
        direction = new Vector2(Player.instance.transform.position.x < transform.position.x ? -1 : Player.instance.transform.position.x > transform.position.x ? 1 : 0, Player.instance.transform.position.y < transform.position.y ? -1 : Player.instance.transform.position.y > transform.position.y ? 1 : 0);
        StartCoroutine(SpawnBullet(direction));
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
        if (Stage.isWallAt(targetPos)) targetPos = transform.position;
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
            yield return null;
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(0.5f, 0.2f);
        Sprite.transform.localPosition = Vector3.zero;
        transform.position = targetPos;

        isMoving = false;
        yield break;
    }

    private IEnumerator SpawnBullet(Vector2 direction)
    {
        // Sync with next beat
        while (!BeatManager.isBeat) yield return null;

        BulletSpawnEffect spawnEffect = PoolManager.Get<BulletSpawnEffect>();
        spawnEffect.source = this;
        spawnEffect.transform.position = new Vector3(transform.position.x + direction.x, transform.position.y + direction.y);
        yield return null;

        // The spawnEffect lasts a beat long
        while (spawnEffect.gameObject.activeSelf) yield return null;

        Bullet bullet = PoolManager.Get<Bullet>();
        bullet.transform.position = new Vector3(spawnEffect.transform.position.x, spawnEffect.transform.position.y);
        bullet.direction = direction * 2f;
        bullet.enemySource = this;
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
            yield return null;
        }
        Vector2 position = Player.instance.transform.position;
        transform.position = position;

        yield return null;
        while (!BeatManager.isBeat)
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
            yield return null;
        }
        AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
        PlayerCamera.TriggerCameraShake(1.5f, 0.3f);
        boxCollider.enabled = true;
        isJumping = false;
        yield break;
    }

    public override bool CanTakeDamage()
    {
        return true;
    }

    public override void TakeDamage(float damage, bool isCritical)
    {
        base.TakeDamage(damage, isCritical);
        UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
    }
}
