using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class Enemy : MonoBehaviour
{
    public bool beatContact;
    public bool isMoving { get; protected set; }

    protected Vector2 direction;
    public bool facingRight { get; protected set; }

    [SerializeField] protected SpriteRenderer Sprite;
    public float SpriteSize = 1f;
    protected float SpriteX = 1f;

    public int MaxHP, CurrentHP;
    public int SpawnIndex;
    public int AItype;

    protected Material spriteRendererMat;
    protected Color emissionColor = new Color(1, 0, 0, 0);
    public bool CanMove()
    {
        if (isMoving) return false;
        else return true;
    }

    public static Enemy GetEnemyOfType(EnemyType enemyType)
    {
        switch (enemyType)
        {
            default:
            case EnemyType.TestEnemy: return PoolManager.Get<TestEnemy>();
        }
    }

    public static int GetEnemyCost(EnemyType enemyType)
    {
        switch (enemyType)
        {
            default:
            case EnemyType.TestEnemy: return 1;
        }
    }

    private void Awake()
    {
        spriteRendererMat = Sprite.material;
        facingRight = true;
        OnInitialize();
    }

    protected abstract void OnInitialize();

    // Update is called once per frame
    void Update()
    {
        OnBehaviourUpdate();
        if (BeatManager.isGameBeat) OnBeat();

        HandleSprite();
    }
    public abstract void OnSpawn();
    protected abstract void OnBehaviourUpdate();

    protected abstract void OnBeat();

    public abstract bool CanTakeDamage();

    private void HandleSprite()
    {
        SpriteSize = Mathf.MoveTowards(SpriteSize, 1f, Time.deltaTime * 4f);
        SpriteX = Mathf.MoveTowards(SpriteX, facingRight ? 1 : -1, Time.deltaTime * 24f);
        Sprite.transform.localScale = new Vector3(SpriteX, 1, 1) * SpriteSize;

        emissionColor = Color.Lerp(emissionColor, new Color(1, 1, 1, 0), Time.deltaTime * 8f);
        spriteRendererMat.SetColor("_EmissionColor", emissionColor);
    }

    public virtual void TakeDamage(int damage)
    {
        if (!CanTakeDamage()) return;

        CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, MaxHP);
        Player.TriggerCameraShake(0.3f, 0.2f);
        AudioController.PlaySound(AudioController.instance.sounds.enemyHurtSound);
        emissionColor = new Color(1, 1, 1, 1);
        UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, damage);

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        AudioController.PlaySound(AudioController.instance.sounds.enemyDeathSound);
        Gem gem = PoolManager.Get<Gem>();
        gem.transform.position = transform.position;
        KillEffect deathFx = PoolManager.Get<KillEffect>();
        deathFx.transform.position = transform.position;
        PoolManager.Return(gameObject, GetType());
        Map.Instance.EnemiesAlive--;
    }
}
