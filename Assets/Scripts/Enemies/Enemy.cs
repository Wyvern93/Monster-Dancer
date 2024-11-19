using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class Enemy : MonoBehaviour
{
    public bool beatContact;
    public bool isElite;
    public bool isMoving { get; protected set; }

    protected Vector2 direction;
    public bool facingRight { get; protected set; }

    [SerializeField] public SpriteRenderer Sprite;
    [SerializeField] protected Rigidbody2D rb;
    public float SpriteSize = 1f;
    protected float SpriteX = 1f;

    public int MaxHP, CurrentHP;
    private int origHealth;
    public int SpawnIndex;
    public int AItype;

    protected Material spriteRendererMat;
    protected Color emissionColor = new Color(1, 0, 0, 0);

    private bool isDead;
    public int experience;
    public int baseExperience;
    public int atk = 1;
    public float speed;

    protected Vector3 velocity;
    protected Vector3 knockback;

    protected Animator animator;

    public Vector3 eventMove;
    public int lifeTime;

    public StunEffect stunStatus;
    public BurningEffect burnStatus;
    public SlownessEffect slownessStatus;
    private float oldAnimSpeed;
    private Vector2 deathDir;
    public bool CanMove()
    {
        if (isMoving) return false;
        if (GameManager.isPaused) return false;
        else return true;
    }

    public virtual bool CanBeStunned(bool isUltimate)
    {
        if (isUltimate) return true;
        if (this is Boss) return false;
        return true;
    }

    public virtual bool CanBeSlowed(bool isUltimate)
    {
        if (isUltimate) return true;
        if (this is Boss) return false;
        return true;
    }

    public bool isCloseEnoughToShoot()
    {
        return (Vector2.Distance(transform.position, Player.instance.GetClosestPlayer(transform.position)) < 9);
    }

    public void OnStun(int time)
    {
        if (!stunStatus.isStunned()) oldAnimSpeed = animator.speed;

        stunStatus.duration = time;
        animator.speed = 0;
        Sprite.color = new Color(0.8f, 0.8f, 1f, 1f);
    }

    public void OnSlow(int time, float amount)
    {
        slownessStatus.duration = time;
        if (slownessStatus.amount < amount) slownessStatus.amount = amount;
    }

    public void OnBurn()
    {
        int time = (int)Player.instance.itemValues["burnDuration"];
        if (!burnStatus.isBurning())
        {
            BurningVisualEffect eff = PoolManager.Get<BurningVisualEffect>();
            eff.source = this;
        }
        if (burnStatus.duration < time) burnStatus.duration = time;
    }

    public void OnStunEnd()
    {
        animator.speed = oldAnimSpeed;
        Sprite.color = Color.white;
    }

    public static Enemy GetEnemyOfType(EnemyType enemyType)
    {
        switch (enemyType)
        {
            default:
            case EnemyType.TestEnemy: return PoolManager.Get<TestEnemy>();
            case EnemyType.BooJr: return PoolManager.Get<GhostJr>();
            case EnemyType.BooJrElite: return PoolManager.Get<GhostJrElite>();
            case EnemyType.CarrotFan: return PoolManager.Get<CarrotFan>();
            case EnemyType.Dancearune: return PoolManager.Get<Dancearune>();
            case EnemyType.DancearuneElite: return PoolManager.Get<DancearuneElite>();
            case EnemyType.NomSlime: return PoolManager.Get<NomSlime>();
            case EnemyType.NomSlimeElite: return PoolManager.Get<NomSlimeElite>();
            case EnemyType.Poisy: return PoolManager.Get<Poisy>();
            case EnemyType.PoisyElite: return PoolManager.Get<PoisyElite>();
            case EnemyType.Skeleko: return PoolManager.Get<Skeleko>();
            case EnemyType.SlimeDancer: return PoolManager.Get<SlimeDancer>();
            case EnemyType.Tronco: return PoolManager.Get<Tronco>();
            case EnemyType.ZombieBride: return PoolManager.Get<ZombieBride>();
            case EnemyType.ZombieThief: return PoolManager.Get<ZombieThief>();
            case EnemyType.UsarinRunning: return PoolManager.Get<UsarinRunning>();

            case EnemyType.ZippyBat: return PoolManager.Get<ZippyBat>();
            case EnemyType.Rhytmia: return PoolManager.Get<Rhythmia>();
            case EnemyType.RhytmiaElite: return PoolManager.Get<RhythmiaElite>();
            case EnemyType.VampiLoli: return PoolManager.Get<VampiLoli>();
            case EnemyType.Fungoo: return PoolManager.Get<Fungoo>();
            case EnemyType.FungooElite: return PoolManager.Get<FungooElite>();
            case EnemyType.Kappa: return PoolManager.Get<Kappa>();
            case EnemyType.MuscleHare: return PoolManager.Get<MuscleHare>();
            case EnemyType.MuscleHareElite: return PoolManager.Get<MuscleHareElite>();
            case EnemyType.OjouGuardian: return PoolManager.Get<OjouGuardian>();
            case EnemyType.Purrfessor: return PoolManager.Get<Purrfessor>();
            case EnemyType.StayinUndead: return PoolManager.Get<StayinUndead>();
            case EnemyType.StayinUndeadElite: return PoolManager.Get<StayinUndeadElite>();
            case EnemyType.WiggleViper: return PoolManager.Get<WiggleViper>();

            case EnemyType.Usarin: return PoolManager.Get<UsarinBoss>();
            case EnemyType.Nebulion: return PoolManager.Get<NebulionBoss>();
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
        stunStatus = new StunEffect();
        burnStatus = new BurningEffect();
        slownessStatus = new SlownessEffect();
        spriteRendererMat = Sprite.material;
        facingRight = true;
        animator = GetComponentInChildren<Animator>();
        OnInitialize();
    }

    private void OnEnable()
    {
        isDead = false;
        Sprite.color = Color.white;
    }

    public virtual void CalculateHealth()
    {
        if (GameManager.runData == null) return;
        if (origHealth == 0) origHealth = MaxHP;
        float minutes = (Map.StageTime / 120f) + (6 * GameManager.runData.stageMulti);
        float scale = 1 + (Mathf.Clamp(minutes - 1, 0, 10000)/4f);
        // Minute 0 -> 1x
        // Minute 10 -> 10x
        int baseHealth = (int)(origHealth * scale);
        MaxHP = baseHealth;

        if (baseExperience == 0) baseExperience = experience;
        experience = baseExperience * (1 + GameManager.runData.stageMulti);

        float baseSize = isElite ? 2 : 1;
        float multi = 1 + (Map.StageTime / 800f);
        transform.localScale = Vector3.one * baseSize * multi;
    }

    private Color ColorMoveTowards(Color a, Color b, float time)
    {
        return new Color(Mathf.MoveTowards(a.r, b.r, time), Mathf.MoveTowards(a.g, b.g, time), Mathf.MoveTowards(a.b, b.b, time), Mathf.MoveTowards(a.a, b.a, time));
    }

    protected abstract void OnInitialize();

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isPaused) return;
        if (Player.instance == null) return;
        if (isDead)
        {
            transform.position += (Vector3)(deathDir * 4 * Time.deltaTime);
            Sprite.color = ColorMoveTowards(Sprite.color, new Color(1, 0, 0, 0), Time.deltaTime * 4f);
            if (Sprite.color.a <= 0f)
            {
                Map.Instance.enemiesAlive.Remove(this);
                PoolManager.Return(gameObject, GetType());
            }
            return;
        }
        if (!stunStatus.isStunned())
        {
            OnBehaviourUpdate();
            if (BeatManager.isGameBeat)
            {
                OnBeat();
            }
        }

        if (burnStatus.isBurning())
        {
            if (BeatManager.isQuarterBeat)
            {
                TakeDamage(2f * Player.instance.itemValues["burnDamage"], false);
            }
            if (BeatManager.isGameBeat)
            {
                burnStatus.duration--;
            }
        }
        else if (BeatManager.isGameBeat && stunStatus.duration > 0)
        {
            if (stunStatus.duration == 1) OnStunEnd();
            stunStatus.duration--;
        }
        if (BeatManager.isGameBeat && slownessStatus.duration > 0)
        {
            slownessStatus.duration--;
        }


        if (stunStatus.isStunned()) rb.velocity = Vector2.zero;
        else
        {
            Vector3 speed = slownessStatus.isSlowed() ? slownessStatus.ApplySlow(velocity.magnitude) * velocity.normalized : velocity;
            rb.velocity = speed + (knockback * 2);
        }
        
        HandleSprite();
        HandleKnockback();
    }
    public virtual void OnSpawn()
    {
        CalculateHealth();
        if (Map.Instance != null) Map.Instance.enemiesAlive.Add(this);

    }

    protected void HandleKnockback()
    {
        if (stunStatus.isStunned()) return;
        knockback = Vector3.MoveTowards(knockback, Vector3.zero, Time.deltaTime * 12f);
    }

    public void PushEnemy(Vector2 pushDir, float force)
    {
        if (this is Boss) return;
        knockback = pushDir.normalized * force;
    }

    protected abstract void OnBehaviourUpdate();

    protected abstract void OnBeat();

    public abstract bool CanTakeDamage();

    private void HandleSprite()
    {
        SpriteSize = Mathf.MoveTowards(SpriteSize, 1f, Time.deltaTime * 4f);
        SpriteX = Mathf.MoveTowards(SpriteX, facingRight ? 1 : -1, Time.deltaTime * 24f);
        Sprite.transform.localScale = new Vector3(SpriteX, 1, 1) * SpriteSize;

        emissionColor = Color.Lerp(emissionColor, new Color(1, 1, 1, 0), Time.deltaTime * 16f);
        spriteRendererMat.SetColor("_EmissionColor", emissionColor);
    }

    public virtual void TakeDamage(float damage, bool isCritical)
    {
        if (isDead) return;
        if (!CanTakeDamage())
        {
            UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, 0, DamageTextType.Dodge);
            return;
        }

        CurrentHP = Mathf.Clamp(CurrentHP - Mathf.RoundToInt(damage), 0, MaxHP);
        //Player.TriggerCameraShake(0.4f, 0.2f);
        AudioController.PlaySound(AudioController.instance.sounds.enemyHurtSound, Random.Range(0.95f, 1.05f));
        emissionColor = new Color(1, 1, 1, 0.5f);
        UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, Mathf.RoundToInt(damage), isCritical ? DamageTextType.Critical : DamageTextType.Normal);

        if (CurrentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    public void ForceDespawn(bool sound = true)
    {
        if (sound)
        {
            //AudioController.PlaySound(AudioController.instance.sounds.enemyDeathSound);
            //KillEffect deathFx = PoolManager.Get<KillEffect>();
            //deathFx.transform.position = transform.position;
        }
        isDead = true;
        deathDir = (transform.position - Player.instance.transform.position).normalized;
    }

    public virtual void Die()
    {
        if (this is not Boss) SpawnDrops();
        if (isElite)
        {
            PlayerCamera.TriggerCameraShake(0.5f, 1f);
            AudioController.PlaySound(AudioController.instance.sounds.bossPhaseEnd, side:true);
            // Should also spawn a chest
        }
        isDead = true;
        deathDir = (transform.position - Player.instance.transform.position).normalized;
        stunStatus.duration = 0;
        burnStatus.duration = 0;
        slownessStatus.duration = 0;
        //KillEffect deathFx = PoolManager.Get<KillEffect>();
        //deathFx.transform.position = transform.position;
        //Map.Instance.enemiesAlive.Remove(this);
        //PoolManager.Return(gameObject, GetType());
    }

    public void SpawnDrops()
    {
        int numSmallGems = experience % 5;
        int numBigGems = (experience - numSmallGems) / 5;

        int numCoins = 0;
        if (Random.Range(0, 40) <= 1)
        {
            numCoins = 1;
            for (int i = 0; i < 20; i++)
            {
                if (Random.Range(0, 40) <= 1) numCoins++;
            }
        }

        // Gather all gems around as experience
        int expToUse = experience;
        
        Collider2D[] gemColliders = Physics2D.OverlapCircleAll(transform.position, 2f);
        List<Gem> gems = new List<Gem>();
        foreach (Collider2D col  in gemColliders)
        {
            Gem gem = col.GetComponent<Gem>();
            if (gem != null) gems.Add(gem);
        }

        if (gems.Count > 2)
        {
            int count = gems.Count - 3;
            experience += gems[0].exp;
            for (int i = 0; i < count; i++)
            {
                gems[i].ForceDespawn();
            }
        }
        /*
        foreach (Collider2D col in gemColliders)
        {
            Gem gem = col.GetComponent<Gem>();
            if (gem == null) continue;

            experience += gem.exp;
            gem.ForceDespawn();
        }*/
        while (expToUse > 0)
        {
            if (expToUse > 25)
            {
                Gem gem = PoolManager.Get<Gem>();
                gem.dir = Random.insideUnitCircle * 2.4f;
                gem.transform.position = transform.position;
                gem.animator.Play("SuperMonsterGem");
                if (expToUse > 50)
                {
                    gem.exp = 50;
                    expToUse -= 50;
                }
                else
                {
                    gem.exp = expToUse;
                    expToUse = 0;
                }
            }
            else if (expToUse <= 25 && expToUse > 10)
            {
                Gem gem = PoolManager.Get<Gem>();
                gem.dir = Random.insideUnitCircle * 2.4f;
                gem.transform.position = transform.position;
                gem.exp = Mathf.Clamp(expToUse, 0, 25);
                expToUse -= expToUse;
                gem.animator.Play("MonsterGem");
            }
            else if (expToUse <= 10)
            {
                BulletGem gem = PoolManager.Get<BulletGem>();
                gem.dir = Random.insideUnitCircle * 2.4f;
                gem.transform.position = transform.position;//(Vector2)transform.position + (Random.insideUnitCircle * 0.4f);
                gem.exp = expToUse;
                expToUse -= expToUse;
            }
        }

        for (int coins = 0; coins < numCoins; coins++)
        {
            Coin coin = PoolManager.Get<Coin>();
            coin.dir = Random.insideUnitCircle * 2.4f;
            coin.transform.position = transform.position;
        }

        int foodChance = Random.Range(0, 60);
        if (foodChance < 1)
        {
            Food food = PoolManager.Get<Food>();
            food.dir = Random.insideUnitCircle * 2.4f;
            food.transform.position = transform.position;
        }

        if (!isElite && Player.instance.hasEvolvableAbility())
        {
            if (Map.isWallAt(new Vector2(transform.position.x, transform.position.y))) return;
            int fairyChance = Random.Range(0, 10);
            if (fairyChance < 1 && !Map.Instance.fairyCage.gameObject.activeSelf)
            {
                Map.Instance.fairyCage.transform.position = transform.position;
                Map.Instance.fairyCage.gameObject.SetActive(true);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (stunStatus.isStunned()) return;
        if (isDead) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (!BeatManager.isGameBeat) return;
        if (isDead) return;
        if (stunStatus.isStunned()) return;

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }
}
