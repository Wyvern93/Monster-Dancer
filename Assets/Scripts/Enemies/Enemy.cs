using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public CircleCollider2D circleCollider;
    [SerializeField] protected string idleAnimation;
    [SerializeField] protected string moveAnimation;
    [SerializeField] protected string shootAnimation;
    protected int beat;
    protected bool isAttacking;
    public bool canBeKnocked = true;

    public bool beatContact;
    public bool isElite;
    protected bool isMoving;

    protected Vector2 direction;
    public bool facingRight { get; protected set; }

    [SerializeField] public SpriteRenderer Sprite;
    [SerializeField] protected Rigidbody2D rb;
    public float SpriteSize = 1f;
    protected float SpriteX = 1f;

    public int MaxHP, CurrentHP;
    public int SpawnIndex;
    public EnemyAIType aiType;

    protected Material spriteRendererMat;
    protected Color emissionColor = new Color(1, 0, 0, 0);

    public bool isDead;
    public int experience;
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

    public EnemyType enemyType;
    public EnemyGroup group;

    public static Dictionary<EnemyType, EnemyData> enemyData = new Dictionary<EnemyType, EnemyData>()
    {
        // Stage 1-1
        {EnemyType.ZombieThief, new EnemyData(EnemyType.ZombieThief, EnemyClass.Runner, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnCircleHordeGroup, StageEventType.SpawnHordeChaseEvent }) },
        {EnemyType.StayinUndead, new EnemyData(EnemyType.StayinUndead, EnemyClass.Runner, EnemyArchetype.Menacer, new List<StageEventType>() { StageEventType.SpawnSpreadGroup}) },
        {EnemyType.ZombieBride, new EnemyData(EnemyType.ZombieBride,EnemyClass.Shooter, EnemyArchetype.Rusher, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnHordeChaseEvent}) },
        {EnemyType.StayinUndeadElite, new EnemyData(EnemyType.StayinUndeadElite, EnemyClass.Elite, EnemyArchetype.Elite, new List<StageEventType>() { StageEventType.SpawnElite}) },
        {EnemyType.Skeleko, new EnemyData(EnemyType.Skeleko, EnemyClass.Runner, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnCircleHordeGroup}) },
        {EnemyType.ClawRiff, new EnemyData(EnemyType.ClawRiff, EnemyClass.Runner, EnemyArchetype.Swarm, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnHordeChaseEvent}) },
        {EnemyType.Nekomander, new EnemyData(EnemyType.Nekomander, EnemyClass.Shooter, EnemyArchetype.Dangerous, new List<StageEventType>() { StageEventType.SpawnGeometricEvent, StageEventType.SpawnSpreadGroup, StageEventType.SpawnCircleHordeGroup}) },
        {EnemyType.NekomanderElite, new EnemyData(EnemyType.NekomanderElite, EnemyClass.Elite, EnemyArchetype.Elite, new List<StageEventType>() { StageEventType.SpawnElite}) },
        {EnemyType.JackO, new EnemyData(EnemyType.JackO, EnemyClass.Shooter, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnGeometricEvent}) },
        {EnemyType.Onibi, new EnemyData(EnemyType.Onibi, EnemyClass.Bomber, EnemyArchetype.Rusher, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnHordeChaseEvent}) },
        {EnemyType.JackOElite, new EnemyData(EnemyType.JackOElite, EnemyClass.Elite, EnemyArchetype.Elite, new List<StageEventType>() { StageEventType.SpawnElite}) },
        {EnemyType.ZippyBat, new EnemyData(EnemyType.ZippyBat, EnemyClass.Bomber, EnemyArchetype.Swarm, new List<StageEventType>() { StageEventType.SpawnGeometricEvent, StageEventType.SpawnCircleHordeGroup }) },
        {EnemyType.RhythMaiden, new EnemyData(EnemyType.RhythMaiden, EnemyClass.Shooter, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnSpreadGroup }) },
        {EnemyType.OjouGuardian, new EnemyData(EnemyType.OjouGuardian, EnemyClass.Runner, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnCircleHordeGroup}) },
        {EnemyType.VampiLoliElite, new EnemyData(EnemyType.VampiLoliElite, EnemyClass.Elite, EnemyArchetype.Elite, new List<StageEventType>() { StageEventType.SpawnElite}) },
        {EnemyType.Usarin, new EnemyData(EnemyType.Usarin, EnemyClass.Boss, EnemyArchetype.Boss, new List<StageEventType>() { StageEventType.SpawnBoss}) },
        // Stage 1-2
        {EnemyType.NomSlime, new EnemyData(EnemyType.NomSlime, EnemyClass.Runner, EnemyArchetype.Swarm, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnCircleHordeGroup}) },
        {EnemyType.Poisy, new EnemyData(EnemyType.Poisy, EnemyClass.Runner, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnHordeChaseEvent}) },
        {EnemyType.SlimeDancer, new EnemyData(EnemyType.SlimeDancer, EnemyClass.Shooter, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnGeometricEvent, StageEventType.SpawnSpreadGroup}) },
        {EnemyType.NomSlimeElite, new EnemyData(EnemyType.NomSlimeElite, EnemyClass.Elite, EnemyArchetype.Elite, new List<StageEventType>() { StageEventType.SpawnElite}) },
        {EnemyType.Tanuki, new EnemyData(EnemyType.Tanuki, EnemyClass.Runner, EnemyArchetype.Tank, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnCircleHordeGroup}) },
        {EnemyType.Fungoo, new EnemyData(EnemyType.Fungoo, EnemyClass.Runner, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnHordeChaseEvent}) },
        {EnemyType.BuzzBee, new EnemyData(EnemyType.BuzzBee, EnemyClass.Runner, EnemyArchetype.Rusher, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnHordeChaseEvent}) },
        {EnemyType.Tronco, new EnemyData(EnemyType.Tronco, EnemyClass.Runner, EnemyArchetype.Juggernaut, new List<StageEventType>() { StageEventType.SpawnSpreadGroup}) },
        {EnemyType.Rhytmia, new EnemyData(EnemyType.Rhytmia, EnemyClass.Shooter, EnemyArchetype.Menacer, new List<StageEventType>() { StageEventType.SpawnGeometricEvent, StageEventType.SpawnHordeChaseEvent, StageEventType.SpawnSpreadGroup}) },
        {EnemyType.Dancearune, new EnemyData(EnemyType.Dancearune, EnemyClass.Shooter, EnemyArchetype.Juggernaut, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnGeometricEvent, StageEventType.SpawnCircleHordeGroup}) },
        {EnemyType.Kappa, new EnemyData(EnemyType.Kappa, EnemyClass.Runner, EnemyArchetype.AllRounder, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnSpreadGroup, StageEventType.SpawnCircleHordeGroup}) },
        {EnemyType.WiggleViper, new EnemyData(EnemyType.WiggleViper, EnemyClass.Shooter, EnemyArchetype.Swarm, new List<StageEventType>() { StageEventType.SpawnSpreadGroup, StageEventType.SpawnHordeChaseEvent}) },
        {EnemyType.Karakasa, new EnemyData(EnemyType.Karakasa, EnemyClass.Shooter, EnemyArchetype.Tank, new List<StageEventType>() { StageEventType.SpawnSpreadGroup }) },
        {EnemyType.FungooElite, new EnemyData(EnemyType.FungooElite, EnemyClass.Elite, EnemyArchetype.Elite, new List<StageEventType>() { StageEventType.SpawnElite}) },
        {EnemyType.RhytmiaElite, new EnemyData(EnemyType.RhytmiaElite, EnemyClass.Elite, EnemyArchetype.Elite, new List<StageEventType>() { StageEventType.SpawnElite}) },
        {EnemyType.DancearuneElite, new EnemyData(EnemyType.DancearuneElite, EnemyClass.Elite, EnemyArchetype.Elite, new List<StageEventType>() { StageEventType.SpawnElite}) },
        {EnemyType.Nebulion, new EnemyData(EnemyType.Nebulion, EnemyClass.Boss, EnemyArchetype.Boss, new List<StageEventType>() { StageEventType.SpawnBoss}) },
    };

    public static EnemyClass enemyClassFromSpawnType(EnemySpawnType enemySpawnType)
    {
        switch (enemySpawnType)
        {
            default:
            case EnemySpawnType.MAIN_RUNNER:
            case EnemySpawnType.SECOND_RUNNER:
            case EnemySpawnType.THIRD_RUNNER:
                return EnemyClass.Runner;
            case EnemySpawnType.MAIN_BOMBER:
            case EnemySpawnType.SECOND_BOMBER:
                return EnemyClass.Bomber;
            case EnemySpawnType.MAIN_SHOOTER:
            case EnemySpawnType.SECOND_SHOOTER:
                return EnemyClass.Shooter;
        }
    }
    public bool CanMove()
    {
        if (GameManager.isPaused) return false;
        if (isMoving) return false;
        if (aiType != EnemyAIType.Orbital)
        {
            if (isAttacking) return false;
        }
        
        return true;
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
        Vector2 camPos = PlayerCamera.instance.transform.position;
        if (transform.position.x > camPos.x + 9) return false;
        if (transform.position.x < camPos.x - 9) return false;
        if (transform.position.y > camPos.y + 5) return false;
        if (transform.position.y < camPos.y - 5) return false;

        return true;
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

    public void OnBurn(PlayerAbility abilitySource, float damage, float duration)
    {
        if (isDead) return;
        if (!burnStatus.isBurning())
        {
            BurningVisualEffect eff = PoolManager.Get<BurningVisualEffect>();
            eff.source = this;
        }
        burnStatus.AddBurn(new BurnSource(damage, duration));
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
            case EnemyType.JackO: return PoolManager.Get<JackO>();
            case EnemyType.JackOElite: return PoolManager.Get<JackOElite>();
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
            case EnemyType.VampiLoliElite: return PoolManager.Get<VampiLoliElite>();
            case EnemyType.Fungoo: return PoolManager.Get<Fungoo>();
            case EnemyType.FungooElite: return PoolManager.Get<FungooElite>();
            case EnemyType.Kappa: return PoolManager.Get<Kappa>();
            case EnemyType.MuscleHare: return PoolManager.Get<MuscleHare>();
            case EnemyType.MuscleHareElite: return PoolManager.Get<NekomanderElite>();
            case EnemyType.OjouGuardian: return PoolManager.Get<OjouGuardian>();
            case EnemyType.Nekomander: return PoolManager.Get<Nekomander>();
            case EnemyType.NekomanderElite: return PoolManager.Get<NekomanderElite>();
            case EnemyType.StayinUndead: return PoolManager.Get<StayinUndead>();
            case EnemyType.StayinUndeadElite: return PoolManager.Get<StayinUndeadElite>();
            case EnemyType.WiggleViper: return PoolManager.Get<WiggleViper>();
            case EnemyType.Onibi: return PoolManager.Get<Onibi>();
            case EnemyType.ClawRiff: return PoolManager.Get<ClawRiff>();
            case EnemyType.RhythMaiden: return PoolManager.Get<RhythMaiden>();
            case EnemyType.BuzzBee: return PoolManager.Get<BuzzBee>();
            case EnemyType.Tanuki: return PoolManager.Get<Tanuki>();
            case EnemyType.Karakasa: return PoolManager.Get<Karakasa>();

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
        canBeKnocked = true;
        isDead = false;
        Sprite.color = Color.white;
        facingRight = Player.instance.transform.position.x > transform.position.x;
    }

    public virtual void CalculateHealth()
    {
        if (GameManager.runData == null) return;


        /*
        if (origHealth == 0) origHealth = MaxHP;
        float minutes = (Stage.StageTime / 120f) + (6 * GameManager.runData.stageMulti);
        float scale = 1 + (Mathf.Clamp(minutes - 1, 0, 10000)/4f);
        // Minute 0 -> 1x
        // Minute 10 -> 10x
        int baseHealth = (int)(origHealth * scale);
        MaxHP = baseHealth;

        if (baseExperience == 0) baseExperience = experience;
        experience = baseExperience * (1 + GameManager.runData.stageMulti);

        float baseSize = isElite ? 2 : 1;
        float multi = 1 + (Stage.StageTime / 800f);*/

        int wave = Stage.Instance.currentWave;

        float baseSize = isElite ? 2 : 1f;
        EnemyData data = enemyData[enemyType];
        ArchetypeStats stats = new ArchetypeStats(data.archetype).getStatsAtWave(wave);
        MaxHP = (int)stats.baseHP;
        CurrentHP = MaxHP;
        atk = (int)stats.baseAttack;
        speed = stats.baseSpeed;

        experience = data.CalculateExperience(wave);

        transform.localScale = Vector3.one * baseSize;
    }

    private Color ColorMoveTowards(Color a, Color b, float time)
    {
        return new Color(Mathf.MoveTowards(a.r, b.r, time), Mathf.MoveTowards(a.g, b.g, time), Mathf.MoveTowards(a.b, b.b, time), Mathf.MoveTowards(a.a, b.a, time));
    }

    protected virtual void OnInitialize() { }

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
                Stage.Instance.enemiesAlive.Remove(this);
                PoolManager.Return(gameObject, GetType());
            }
            return;
        }
        if (!stunStatus.isStunned())
        {
            OnBehaviourUpdate();
            if (BeatManager.isBeat)
            {
                OnBeat();
            }
        }
        if (BeatManager.isQuarterBeat)
        {
            float burnDamage = burnStatus.OnBurnTick();

            if (burnStatus.isBurning())
            {
                TakeDamage(burnDamage, false);
            }
        }
        
        if (BeatManager.isBeat && stunStatus.duration > 0)
        {
            if (stunStatus.duration == 1) OnStunEnd();
            stunStatus.duration--;
        }
        if (BeatManager.isBeat && slownessStatus.duration > 0)
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
        if (Stage.Instance != null) Stage.Instance.enemiesAlive.Add(this);

        CurrentHP = MaxHP;
        emissionColor = new Color(1, 1, 1, 0);
        isMoving = false;
        Sprite.transform.localPosition = Vector3.zero;
        animator.Play(idleAnimation);
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2;
        beat = BeatManager.beats % 8;
        isAttacking = false;
    }

    protected void HandleKnockback()
    {
        if (stunStatus.isStunned()) return;
        knockback = Vector3.MoveTowards(knockback, Vector3.zero, Time.deltaTime * 12f);
    }

    public void PushEnemy(Vector2 pushDir, float force)
    {
        if (this is Boss) return;
        if (!canBeKnocked) return;
        knockback = pushDir.normalized * force;
    }

    protected virtual void OnBehaviourUpdate() { }

    protected virtual void OnBeat()
    {
        switch (enemyData[enemyType].enemyClass)
        {
            case EnemyClass.Runner:
            case EnemyClass.Bomber:
                if (CanMove()) StartCoroutine(JumpCoroutine());
                break;
            case EnemyClass.Shooter:
                beat--;
                if (beat > 0)
                {
                    if (CanMove()) StartCoroutine(JumpCoroutine());
                    break;
                }
                else if (beat <= 0)
                {
                    if (isCloseEnoughToShoot())
                    {
                        if (aiType == EnemyAIType.Orbital) StartCoroutine(JumpCoroutine());
                        Shoot();
                    }
                    else if (aiType == EnemyAIType.Orbital) StartCoroutine(JumpCoroutine()); 
                    else if (CanMove())
                    {
                        StartCoroutine(JumpCoroutine());
                    }
                    beat = 8;
                    break;
                }
                break;
        }
    }

    protected Vector2 WithStageVelocity(Vector2 velocity)
    {
        float speedMargin = 0.1f;
        // Sum the velocities
        Vector2 StageVelocity = PlayerCamera.instance.camVelocity;
        float minimumSpeed = PlayerCamera.instance.camVelocity.magnitude + speedMargin;
        float targetSpeed = Mathf.Max(speed, minimumSpeed);
        return velocity.normalized * targetSpeed;
        /*
        Vector2 differenceVector = StageVelocity.normalized - velocity.normalized;
        float difference = differenceVector.magnitude * 2; // So it can go against the velocity too
        float velocityAdded = Mathf.Clamp(1 - difference, -1, 1) * StageVelocity.magnitude;
        float velocityMagnitude = velocity.magnitude;
        velocity = velocity.normalized * (velocityMagnitude + velocityAdded);
        return velocity;*/
    }

   protected virtual IEnumerator JumpCoroutine()
    {
        isMoving = true;
        
        Vector2 dir = GetDirectionFromAI();

        facingRight = dir.x > 0;
        animator.Play(moveAnimation);
        animator.speed = 1f / BeatManager.GetBeatDuration() * 1.5f;
        //animator.speed *= 2;

        float time = 0;
        float spd = 6;

        if (aiType == EnemyAIType.CircleHorde) spd += 0.2f;
        if (dir.x > 0) speed += (dir.x * Time.deltaTime);
        velocity = dir * speed * spd;
        velocity = WithStageVelocity(velocity);

        float beatDuration = BeatManager.GetBeatDuration() * 0.5f;
        float beatTime = 1;

        while (time <= BeatManager.GetBeatDuration() * 0.5f)
        {
            while (GameManager.isPaused || stunStatus.isStunned()) yield return null;

            float beatProgress = time / beatDuration;
            beatTime = Mathf.SmoothStep(1, 0f, beatProgress);
            if (aiType == EnemyAIType.Orbital)
            {
                float anglePerEnemy = 360f / group.totalEnemies;
    
                float currentAngle = group.orbitAngle + (anglePerEnemy * SpawnIndex);

                Vector3 center = group.GetCenter();
                Vector3 targetPosition = new Vector3(center.x + (Mathf.Cos(currentAngle * Mathf.Deg2Rad) * group.orbitDistance), center.y + (Mathf.Sin(currentAngle * Mathf.Deg2Rad) * group.orbitDistance));
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * spd * beatTime * Time.deltaTime);
                velocity = Vector3.zero;
            }
            else
            {
                //dir += group.dirToPlayer;
                //dir.Normalize();
                velocity = dir * speed * spd * beatTime * 1.3f;
            }
            time += Time.deltaTime;
            yield return null;
        }
        if (isElite)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossWalk);
            PlayerCamera.TriggerCameraShake(0.5f, 0.2f);
        }
        animator.Play(idleAnimation);
        animator.speed = 1f / BeatManager.GetBeatDuration();
        velocity = Vector2.zero;
        Sprite.transform.localPosition = Vector3.zero;

        isMoving = false;
        yield break;
    }

    protected Vector2 GetDirectionFromAI()
    {
        Vector3 playerPos = Player.instance.transform.position;
        switch (aiType)
        {
            default:
            case EnemyAIType.Spread:
                return (playerPos - transform.position).normalized;
            case EnemyAIType.CircleHorde:
                return group.dirToPlayer; //The direction is assigned on spawn and on reset from the group
            case EnemyAIType.HordeChase:
                return group.dirToPlayer;
            case EnemyAIType.Orbital:
                return (playerPos - transform.position).normalized;

        }
    }
    
    protected Vector2 CalculateOrbitalDir()
    {
        // Direction from group to enemy
        Vector3 groupPos = group.GetCenter();
        Vector2 groupDir = (transform.position - groupPos).normalized;

        // Get Center Angle to Enemy
        float centerAngle = BulletBase.VectorToAngle(groupDir);

        // Sum 10º
        float orbitSize = 2 * Mathf.PI * group.orbitDistance; // circumference
        float perAngleSize = orbitSize / 360f;
        float angleEnemyCanMove = perAngleSize * speed;
        if (!GameManager.isPaused) centerAngle += angleEnemyCanMove * Time.deltaTime;

        // Get Position in orbit with that angle
        Vector2 targetPosition = new Vector2(groupPos.x + (Mathf.Cos(centerAngle * Mathf.Deg2Rad) * group.orbitDistance), groupPos.y + (Mathf.Sin(centerAngle * Mathf.Deg2Rad) * group.orbitDistance));

        // Get the direction from enemy to the new angle position
        Vector2 enemyDirection = (targetPosition - (Vector2)transform.position).normalized;

        // Return direction
        return enemyDirection;
    }

    protected virtual void Shoot() { }

    public virtual bool CanTakeDamage()
    {
        return true;
    }

    private void HandleSprite()
    {
        SpriteSize = Mathf.MoveTowards(SpriteSize, 1f, Time.deltaTime * 4f);
        SpriteX = Mathf.MoveTowards(SpriteX, facingRight ? 1 : -1, Time.deltaTime * 24f);
        Sprite.transform.localScale = new Vector3(SpriteX, 1, 1) * SpriteSize;

        emissionColor = Color.Lerp(emissionColor, new Color(1, 1, 1, 0), Time.deltaTime * 24f);
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
        AudioController.PlaySoundOnBeat(AudioController.instance.sounds.enemyHurtSound, Random.Range(0.95f, 1.05f));
        emissionColor = new Color(1, 1, 1, 1f);
        UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, Mathf.RoundToInt(damage), isCritical ? DamageTextType.Critical : DamageTextType.Normal);

        if (isElite)
        {
            UIManager.Instance.PlayerUI.UpdateBossBar(CurrentHP, MaxHP);
        }
        if (CurrentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    public void ForceDespawn(bool sound = true)
    {
        if (this is Boss) return;
        if (sound)
        {
            //AudioController.PlaySound(AudioController.instance.sounds.enemyDeathSound);
            //KillEffect deathFx = PoolManager.Get<KillEffect>();
            //deathFx.transform.position = transform.position;
        }

        stunStatus.duration = 0;
        burnStatus.Clear();
        slownessStatus.duration = 0;

        if (isDead) return;
        if (isElite)
        {
            Debug.Log("elite defeated");
            Stage.Instance.elitesDefeated++;
            Stage.Instance.playingWave.isFinalized = true;
            Stage.Instance.currentStagePoint.OnEventFinish();
            Stage.Instance.isEliteAlive = false;
            Stage.Instance.canSpawnEnemies = true;
            UIManager.Instance.PlayerUI.ShowBossBar(false);
        }
        else
        {
            group.enemies.Remove(this);
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
            Stage.Instance.elitesDefeated++;
            Stage.Instance.playingWave.isFinalized = true;
            Stage.Instance.currentStagePoint.OnEventFinish();
            Stage.Instance.isEliteAlive = false;
            UIManager.Instance.PlayerUI.ShowBossBar(false);
            Stage.Instance.canSpawnEnemies = true;
            // Should also spawn a chest
        }
        isDead = true;
        deathDir = (transform.position - Player.instance.transform.position).normalized;
        stunStatus.duration = 0;
        burnStatus.Clear();
        slownessStatus.duration = 0;
        if (!isElite && group != null) group.enemies.Remove(this);

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
            if (Stage.isWallAt(new Vector2(transform.position.x, transform.position.y))) return;
            int fairyChance = Random.Range(0, 10);
            if (fairyChance < 1 && !Stage.Instance.fairyCage.gameObject.activeSelf)
            {
                Stage.Instance.fairyCage.transform.position = transform.position;
                Stage.Instance.fairyCage.gameObject.SetActive(true);
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
        if (!BeatManager.isBeat) return;
        if (isDead) return;
        if (stunStatus.isStunned()) return;

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }
}
