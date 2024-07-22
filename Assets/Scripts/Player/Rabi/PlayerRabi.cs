using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRabi : Player
{
    [SerializeField] Rigidbody2D rb;

    [Header("Ability Prefabs")]
    [SerializeField] GameObject explosiveCarrot;
    [SerializeField] GameObject carrotExplosionPrefab;
    [SerializeField] GameObject rabiClonePrefab;
    [SerializeField] GameObject smokeExplosionPrefab;

    [SerializeField] GameObject moonBeamPrefab;
    [SerializeField] GameObject orbitalMoonPrefab;

    [SerializeField] GameObject carrotJuicePrefab;
    [SerializeField] GameObject carrotJuiceBottlePrefab;
    [SerializeField] GameObject lunarPulsePrefab;
    [SerializeField] GameObject lunarRainBeamPrefab;
    [SerializeField] GameObject truckPrefab;
    [SerializeField] GameObject carrotBulletPrefab;

    [SerializeField] SpriteTrail spriteTrail;
    [SerializeField] CircleCollider2D dashHitBox;
    [SerializeField] AudioClip dashSound;


    public bool isCastingBunnyHop;
    public bool isCastingMoonBeam;

    protected override void Awake()
    {
        base.Awake();

        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 2);
        animator.Play("Rabi_Idle");
    }

    public override void Start()
    {
        base.Start();
        PoolManager.CreatePool(typeof(RabiAttack), attackPrefab, 20);
        PoolManager.CreatePool(typeof(ExplosiveCarrot), explosiveCarrot, 120);
        PoolManager.CreatePool(typeof(SmokeExplosion), smokeExplosionPrefab, 10);
        PoolManager.CreatePool(typeof(RabiClone), rabiClonePrefab, 10);
        PoolManager.CreatePool(typeof(CarrotExplosion), carrotExplosionPrefab, 120);
        PoolManager.CreatePool(typeof(MoonBeam), moonBeamPrefab, 4);
        PoolManager.CreatePool(typeof(OrbitalMoon), orbitalMoonPrefab, 12);
        PoolManager.CreatePool(typeof(CarrotJuice), carrotJuicePrefab, 12);
        PoolManager.CreatePool(typeof(CarrotJuiceBottle), carrotJuiceBottlePrefab, 12);
        PoolManager.CreatePool(typeof(LunarPulse), lunarPulsePrefab, 12);
        PoolManager.CreatePool(typeof(LunarRainRay), lunarRainBeamPrefab, 30);
        PoolManager.CreatePool(typeof(CarrotDeliveryTruck), truckPrefab, 20);
        PoolManager.CreatePool(typeof(CarrotBullet), carrotBulletPrefab, 150);

        GameManager.runData.possibleSkillEnhancements = new List<Enhancement>()
        { 
            new MoonlightDaggersEnhancement(),
            new MoonlightDaggersMasterfulEnhancement(),
            new MoonlightDaggersPowerfulEnhancement(),
            new CarrotBarrageAbilityEnhancement(),
            new BunnyHopAbilityEnhancement(),
            new MoonBeamAbilityEnhancement(),
            new OrbitalMoonAbilityEnhancement(),
            new CarrotJuiceAbilityEnhancement(),
            new LunarPulseAbilityEnhancement(),
            new RabbitReflexesAbilityEnhancement(),
            new LunarRainAbilityEnhancement(),
            new IllusionDashAbilityEnhancement(),
            new CarrotDeliveryAbilityEnhancement()
        };
        
        //LunarPulseAbility orbitalmoon = new LunarPulseAbility();
        //orbitalmoon.OnEquip();
        //equippedPassiveAbilities.Add(orbitalmoon);
        //abilityValues.Add("ability.lunarpulse.level", 1);
        
        //ultimateAbility = new CarrotDeliveryAbility();
        //ultimateAbility.OnEquip();
        //instance.abilityValues.Add("ability.carrotdelivery.level", 1);
        //CurrentSP = MaxSP;

        abilityValues.Add("Attack_Number", 2); // 4 Upgrades
        abilityValues.Add("Attack_Size", 1f); // 20 Upgrades
        abilityValues.Add("Attack_Velocity", 1); // 20 Upgrades
        abilityValues.Add("Attack_Time", 1f); // 10 Upgrades

        abilityValues.Add("Max_Attack_Number", 10);
        abilityValues.Add("Max_Attack_Size", 3);
        abilityValues.Add("Max_Attack_Velocity", 3);
        abilityValues.Add("Max_Attack_Time", 4);

        MoonlightDaggersEnhancement attack = new MoonlightDaggersEnhancement();
        attack.OnEquip();
    }

    public override void Despawn()
    {
        PoolManager.RemovePool(typeof(RabiAttack));
        PoolManager.RemovePool(typeof(ExplosiveCarrot));
        PoolManager.RemovePool(typeof(CarrotExplosion));
        PoolManager.RemovePool(typeof(RabiClone));
        PoolManager.RemovePool(typeof(SmokeExplosion));
        PoolManager.RemovePool(typeof(MoonBeam));
        PoolManager.RemovePool(typeof(OrbitalMoon));
        PoolManager.RemovePool(typeof(CarrotJuice));
        PoolManager.RemovePool(typeof(CarrotJuiceBottle));
        PoolManager.RemovePool(typeof(LunarPulse));
        PoolManager.RemovePool(typeof(LunarRainRay));
        PoolManager.RemovePool(typeof(CarrotDeliveryTruck));
        PoolManager.RemovePool(typeof(CarrotBullet));

        Destroy(gameObject);
    }

    public override List<Enhancement> GetAttackEnhancementList() 
    {
        return new List<Enhancement>()
        {
            new MoonlightDaggersEnhancement(),
            new MoonlightDaggersMasterfulEnhancement(),
            new MoonlightDaggersPowerfulEnhancement()
        };
    }

    protected override IEnumerator DeathCoroutine()
    {
        BeatManager.Stop();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Sprite.sortingLayerName = "UI";
        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        animator.Play("Rabi_Dead");

        UIManager.Instance.PlayerUI.HideUI();
        yield return new WaitForEndOfFrame();
        Time.timeScale = 0.01f;

        UIManager.Instance.SetGameOverBG(true);
        // Play death animation

        yield return new WaitForSecondsRealtime(1f);
        UIManager.Instance.StartGameOverScreen();
        AudioController.PlayMusic(AudioController.instance.gameOverFanfare);
        yield break;
    }

    protected override IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        isMoving = true;
        SpriteSize = 1.2f;
        originPos = transform.position;
        float time = 0;
        if (direction == Vector2.zero)
        {
            direction = (oldDir * currentStats.Speed);
            targetPos = (Vector2)originPos + (oldDir * currentStats.Speed);
        }
        Vector2 dir = Vector2.zero;
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
        {
            dir.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            dir.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
        }
        else
        {
            Vector2 leftStick = InputManager.GetLeftStick();
            dir = leftStick;
        }
        if (dir == Vector2.zero)
        {
            dir.x = facingRight ? 1 : -1;
        }
        dir.Normalize();

        animator.Play("Rabi_Move");
        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 1.5f);

        while (time <= BeatManager.GetBeatDuration() / 3f)
        {
            if (isCastingBunnyHop)
            {
                rb.velocity = Vector2.zero;
                yield break;
            }
            
            //if (Map.isWallAt(targetPos)) targetPos = originPos;

            //transform.position = Vector3.Lerp(originPos, (Vector3)targetPos, time * 8f);
            rb.velocity = dir * currentStats.Speed * 8;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rb.velocity = Vector2.zero;
        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 2);
        animator.Play("Rabi_Idle");
        yield return new WaitForEndOfFrame();
        Sprite.transform.localPosition = Vector3.zero;

        isPerformingAction = false;
        isMoving = false;
        yield break;
    }

    public void DoIllusionDash(int tiles)
    {
        Vector2 dir = Vector2.zero;
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
        {
            dir.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            dir.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
        }
        else
        {
            Vector2 leftStick = InputManager.GetLeftStick();
            dir = leftStick;
        }
        if (dir == Vector2.zero)
        {
            dir.x = facingRight ? 1 : -1;
        }
        dir.Normalize();
        isCastingBunnyHop = true;
        direction = dir;
        isPerformingAction = true;
        StartCoroutine(IllusionDashCoroutine(dir, tiles));
    }

    private IEnumerator IllusionDashCoroutine(Vector2 dir, int tiles)
    {
        isMoving = true;
        originPos = transform.position;

        int finalDistance = 0;
        Vector2 targetPos;

        dashHitBox.enabled = true;
        targetPos = originPos + (dir * finalDistance);
        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 4);
        animator.Play("Rabi_Dash");
        spriteTrail.Play(Sprite, 25, 0.005f, Sprite.transform, new Color(0, 1, 1, 0.5f));
        AudioController.PlaySound(dashSound);

        float time = 0;
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            rb.velocity = dir * currentStats.Speed * 16;
            //transform.position = Vector3.Lerp(originPos, (Vector3)targetPos, time * 6f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rb.velocity = Vector2.zero;

        spriteTrail.Stop();
        dashHitBox.enabled = false;
        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 2);
        animator.Play("Rabi_Idle");
        Sprite.transform.localPosition = Vector3.zero;

        isPerformingAction = false;
        isCastingBunnyHop = false;
        isMoving = false;

        yield break;
    }
    

    public override void OnAttack()
    {
        if (isCastingMoonBeam) return;
        if (UIManager.Instance.PlayerUI.crosshair.transform.position.x > transform.position.x) facingRight = true;
        else facingRight = false;
        StartCoroutine(AttackCoroutine());
    }

    public override void Move(Vector2 targetPos)
    {
        if (isMoving) return;
        BeatManager.OnPlayerAction();
        if (isCastingBunnyHop) return;
        StartCoroutine(MoveCoroutine(targetPos));
    }

    public void DoBunnyHop()
    {
        Vector2 dir = Vector2.zero;
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
        {
            dir.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            dir.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
        }
        else
        {
            Vector2 leftStick = InputManager.GetLeftStick();
            dir.x = leftStick.x > 0.4f ? 1 : leftStick.x < -0.4f ? -1 : 0;
            dir.y = leftStick.y > 0.4f ? 1 : leftStick.y < -0.4f ? -1 : 0;
        }
        if (dir == Vector2.zero) dir = facingRight ? Vector2.right : Vector2.left;
        dir *= 2;
        isCastingBunnyHop = true;
        direction = dir;
        StartCoroutine(BunnyHopCoroutine((Vector2)transform.position + dir));
        
    }

    private IEnumerator BunnyHopCoroutine(Vector2 targetPos)
    {
        isMoving = true;
        bool hasSpawnedClone = false;
        SpriteSize = 1.2f;
        originPos = transform.position;
        float time = 0;
        if (direction == Vector2.zero)
        {
            direction = oldDir * 2;
            targetPos = (Vector2)originPos + oldDir;
        }
        if (Map.isWallAt(targetPos)) targetPos = originPos;

        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 2);
        animator.Play("Rabi_BunnyHop");
        while (time <= BeatManager.GetBeatDuration())
        {
            if (Map.isWallAt(targetPos)) targetPos = originPos;

            if (time >= BeatManager.GetBeatDuration() / 3f && !hasSpawnedClone)
            {
                hasSpawnedClone = true;
                SmokeExplosion smokeExplosion = PoolManager.Get<SmokeExplosion>();
                smokeExplosion.transform.position = originPos;

                RabiClone rabiClone = PoolManager.Get<RabiClone>();
                rabiClone.transform.position = originPos;
                rabiClone.OnInit();
                playerClones.Add(rabiClone.gameObject);

            }
            transform.position = Vector3.Lerp(originPos, (Vector3)targetPos, time * 3f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = targetPos;
        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 2);
        animator.Play("Rabi_Idle");
        Sprite.transform.localPosition = Vector3.zero;
        transform.position = targetPos;

        isPerformingAction = false;
        isCastingBunnyHop = false;
        isMoving = false;
        yield break;
    }

    private IEnumerator AttackCoroutine()
    {
        int numAttacks = 2;
        int attackLevel = (int)abilityValues["Attack_Number"];

        numAttacks = attackLevel;

        float attackduration = 0;
        float baseBeat = BeatManager.GetBeatDuration();

        attackduration = baseBeat / (numAttacks * 2);
        if (numAttacks == 2) attackduration = baseBeat / 4f;
        if (numAttacks == 4) attackduration = baseBeat / 8f;
        if (numAttacks == 6) attackduration = baseBeat / 12f;

        float remainingAttacks = numAttacks;
        while (remainingAttacks > 0)
        {
            PlayerAttack atkEntity = PoolManager.Get<RabiAttack>();
            atkEntity.Attack(direction);
            atkEntity.transform.localScale = Vector3.one * abilityValues["Attack_Size"];
            remainingAttacks--;
            yield return new WaitForSeconds(attackduration);
        }
        yield break;
    }

    public override void OnPassiveAbility1Use()
    {
        if (equippedPassiveAbilities[0].CanCast()) equippedPassiveAbilities[0].OnCast();
    }

    public override void OnPassiveAbility2Use()
    {
        if (equippedPassiveAbilities[1].CanCast()) equippedPassiveAbilities[1].OnCast();
    }
    public override void OnPassiveAbility3Use()
    {
        if (equippedPassiveAbilities[2].CanCast()) equippedPassiveAbilities[2].OnCast();
    }

    public override void OnActiveAbilityUse() 
    {
        activeAbility.OnCast();
    }

    public override void OnUltimateUse()
    {
        if (ultimateAbility.CanCast()) ultimateAbility.OnCast();
    }

    public override void TakeDamage(int damage)
    {
        if (isCastingBunnyHop) return;
        if (isInvulnerable) return;
        if (isDead) return;

        RabbitReflexesAbility reflexes = (RabbitReflexesAbility)equippedPassiveAbilities.FirstOrDefault(x=> x.GetType() == typeof(RabbitReflexesAbility));
        if (reflexes != null)
        {
            reflexes.OnDamage();
        }

        CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, 9999);
        TriggerCameraShake(0.3f, 0.2f);
        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.DoHurtEffect();
        AudioController.PlaySound(AudioController.instance.sounds.playerHurtSfx);

        UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, damage, DamageTextType.PlayerDamage);

        emissionColor = new Color(1, 0, 0, 1);

        if (CurrentHP <= 0)
        {
            Die();
        }
    }
}
