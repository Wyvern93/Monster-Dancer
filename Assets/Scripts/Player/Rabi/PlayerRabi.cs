using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRabi : Player
{

    [Header("Ability Prefabs")]
    [SerializeField] GameObject moonlightDaggerWavePrefab;
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
    [SerializeField] GameObject moonlightShockwavePrefab;
    [SerializeField] GameObject rabiEclipsePrefab;
    [SerializeField] GameObject piercingShotPrefab;
    [SerializeField] GameObject carrotBusterPrefab;
    [SerializeField] GameObject carrotcopterPrefab;
    [SerializeField] GameObject moonlightFlowerPrefab;
    [SerializeField] GameObject lunarAuraPrefab;

    [SerializeField] GameObject juiceExplosionPrefab;

    [SerializeField] GameObject sanctuaryPrefab;
    [SerializeField] GameObject sanctuaryPulsePrefab;
    [SerializeField] GameObject holyOrbPrefab;

    [SerializeField] GameObject flamingDrillPrefab;
    [SerializeField] GameObject flamingDrillBlastPrefab;
    [SerializeField] GameObject groundFirePrefab;

    [SerializeField] GameObject fireworkPrefab;
    [SerializeField] GameObject fireworkExplosionPrefab;

    [SerializeField] SpriteTrail spriteTrail;
    [SerializeField] CircleCollider2D dashHitBox;
    [SerializeField] AudioClip dashSound;
    [SerializeField] public AudioClip throwSound;
    [SerializeField] public AudioClip piercingShotChargeSound;
    [SerializeField] public AudioClip flamingDrillChargeSound;
    [SerializeField] public AudioClip fireworkSound;

    public bool isCastingBunnyHop;

    protected override void Awake()
    {
        base.Awake();

        //animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        animator.speed = 1f / BeatManager.GetBeatDuration();
        animator.Play("Rabi_Idle");
    }

    public override void Start()
    {
        base.Start();
        //PoolManager.CreatePool(typeof(RabiAttack), attackPrefab, 20);
        PoolManager.CreatePool(typeof(MoonlightDaggerWave), moonlightDaggerWavePrefab, 12);
        PoolManager.CreatePool(typeof(ExplosiveCarrot), explosiveCarrot, 120);
        PoolManager.CreatePool(typeof(SmokeExplosion), smokeExplosionPrefab, 100);
        PoolManager.CreatePool(typeof(CarrotExplosion), carrotExplosionPrefab, 120);
        PoolManager.CreatePool(typeof(MoonbeamLaser), moonBeamPrefab, 4);
        PoolManager.CreatePool(typeof(CarrotJuice), carrotJuicePrefab, 12);
        PoolManager.CreatePool(typeof(CarrotJuiceBottle), carrotJuiceBottlePrefab, 4);
        PoolManager.CreatePool(typeof(LunarRainRay), lunarRainBeamPrefab, 30);
        PoolManager.CreatePool(typeof(CarrotDeliveryTruck), truckPrefab, 20);
        PoolManager.CreatePool(typeof(CarrotBullet), carrotBulletPrefab, 150);
        PoolManager.CreatePool(typeof(MoonlightShockwave), moonlightShockwavePrefab, 30);
        PoolManager.CreatePool(typeof(RabiEclipse), rabiEclipsePrefab, 1);
        PoolManager.CreatePool(typeof(CarrotBuster), carrotBusterPrefab, 1);
        PoolManager.CreatePool(typeof(CarrotCopter), carrotcopterPrefab, 8);
        PoolManager.CreatePool(typeof(MoonlightFlower), moonlightFlowerPrefab, 8);
        PoolManager.CreatePool(typeof(JuiceExplosion), juiceExplosionPrefab, 5);

        PoolManager.CreatePool(typeof(SanctuaryAura), sanctuaryPrefab, 1);
        PoolManager.CreatePool(typeof(SanctuaryPulse), sanctuaryPulsePrefab, 1);
        PoolManager.CreatePool(typeof(HolyOrb), holyOrbPrefab, 50);
        PoolManager.CreatePool(typeof(FlamingDrill), flamingDrillPrefab, 10);
        PoolManager.CreatePool(typeof(FlamingDrillFlames), flamingDrillBlastPrefab, 10);
        PoolManager.CreatePool(typeof(GroundFire), groundFirePrefab, 100);

        PoolManager.CreatePool(typeof(Firework), fireworkPrefab, 10);
        PoolManager.CreatePool(typeof(FireworkExplosion), fireworkExplosionPrefab, 30);

        GameManager.runData.possibleSkillEnhancements = new List<Enhancement>()
        { 
            new MoonlightDaggersEnhancement(),
            new CarrotBarrageAbilityEnhancement(),
            new MoonBeamAbilityEnhancement(),
            new CarrotJuiceAbilityEnhancement(),
            new LunarRainAbilityEnhancement(),
            new CarrotDeliveryAbilityEnhancement(),
            new EclipseAbilityEnhancement(),
            new CarrotBusterAbilityEnhancement(),
            new CarrotcopterAbilityEnhancement(),
            new MoonlightFlowerAbilityEnhancement()
        };
        
        BunnyHopAbility hop = new BunnyHopAbility();
        hop.OnEquip();
        activeAbility = hop;
        //equippedPassiveAbilities.Add(orbitalmoon);
        abilityValues.Add("ability.bunnyhop.level", 1);
        UIManager.Instance.PlayerUI.activeCDImage.fillAmount = 0;
        activeAbility.currentCooldown = 0;

        //ultimateAbility = new CarrotDeliveryAbility();
        //ultimateAbility.OnEquip();
        //instance.abilityValues.Add("ability.carrotdelivery.level", 1);
        //CurrentSP = MaxSP;

        abilityValues.Add("Attack_Number", 2); // 4 Upgrades
        abilityValues.Add("Attack_Size", 1f); // 20 Upgrades
        abilityValues.Add("Attack_Velocity", 1); // 20 Upgrades
        abilityValues.Add("Attack_Time", 1f); // 10 Upgrades
        abilityValues.Add("Attack_Cooldown", 4);
        abilityValues.Add("Attack_Pierce", 3);
        abilityValues.Add("Attack_Spread", 0);
        abilityValues.Add("Attack_Explode", 0);

        // Moonlight Daggers Spell-Shot
        MoonlightDaggersEnhancement attack = new MoonlightDaggersEnhancement();
        attack.OnEquip();
    }

    public override void Despawn()
    {
        PoolManager.RemovePool(typeof(MoonlightDaggerWave));
        PoolManager.RemovePool(typeof(ExplosiveCarrot));
        PoolManager.RemovePool(typeof(CarrotExplosion));
        PoolManager.RemovePool(typeof(SmokeExplosion));
        PoolManager.RemovePool(typeof(MoonbeamLaser));
        PoolManager.RemovePool(typeof(CarrotJuice));
        PoolManager.RemovePool(typeof(CarrotJuiceBottle));
        PoolManager.RemovePool(typeof(LunarRainRay));
        PoolManager.RemovePool(typeof(CarrotDeliveryTruck));
        PoolManager.RemovePool(typeof(CarrotBullet));
        PoolManager.RemovePool(typeof(RabiEclipse));
        PoolManager.RemovePool(typeof(CarrotBuster));
        PoolManager.RemovePool(typeof(CarrotCopter));
        PoolManager.RemovePool(typeof(MoonlightFlower));
        PoolManager.RemovePool(typeof(JuiceExplosion));

        PoolManager.RemovePool(typeof(SanctuaryAura));
        PoolManager.RemovePool(typeof(SanctuaryPulse));
        PoolManager.RemovePool(typeof(HolyOrb));

        PoolManager.RemovePool(typeof(FlamingDrill));
        PoolManager.RemovePool(typeof(FlamingDrillFlames));
        PoolManager.RemovePool(typeof(GroundFire));

        PoolManager.RemovePool(typeof(Firework));
        PoolManager.RemovePool(typeof(FireworkExplosion));

        spriteTrail.Stop();
        spriteTrail.ForceDespawn();
        Destroy(gameObject);
    }

    public override void ResetAbilities()
    {
        isCastingBunnyHop = false;
        spriteTrail.Stop();
    }

    public override List<Enhancement> GetAttackEnhancementList() 
    {
        return new List<Enhancement>()
        {
            new MoonlightDaggersEnhancement(),
        };
    }

    protected override IEnumerator DeathCoroutine()
    {
        BeatManager.Stop();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Sprite.sortingLayerName = "UI";
        //animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        animator.speed = 1f / BeatManager.GetBeatDuration();
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
        }

        Vector2 dir = Vector2.zero;

        animator.Play("Rabi_Move");
        animator.speed = 1f / BeatManager.GetBeatDuration() / 0.8f;

        while (time <= BeatManager.GetBeatDuration() * 0.8f)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            if (time <= 0.05f)
            {
                Vector2 tempDir;
                if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
                {
                    tempDir.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
                    tempDir.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
                }
                else
                {
                    Vector2 leftStick = InputManager.GetLeftStick();
                    tempDir = leftStick;
                }
                if (tempDir != Vector2.zero) dir = tempDir;
                dir.Normalize();

            }
            bool isCrouching = Keyboard.current.leftShiftKey.isPressed;
            if (isCastingBunnyHop)
            {
                rb.velocity = Vector2.zero;
                yield break;
            }
            
            rb.velocity = dir * currentStats.Speed * (isCrouching ? 2f : 4);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rb.velocity = Vector2.zero;
        animator.speed = 1f / BeatManager.GetBeatDuration();
        animator.Play("Rabi_Idle");
        yield return new WaitForEndOfFrame();
        Sprite.transform.localPosition = Vector3.zero;

        isPerformingAction = false;
        isMoving = false;
        yield break;
    }

    public void DoIllusionDash(int level)
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
        StartCoroutine(IllusionDashCoroutine(dir, level));
    }

    private IEnumerator IllusionDashCoroutine(Vector2 dir, int level)
    {
        isMoving = true;
        originPos = transform.position;

        float dashSpeed = level < 4 ? 10 : 12f;

        dashHitBox.enabled = true;
        //animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 4);
        animator.speed = 1f / BeatManager.GetBeatDuration() / 4f;
        animator.Play("Rabi_Dash");
        spriteTrail.Play(Sprite, 10, 0.05f, Sprite.transform, new Color(0, 1, 1, 0.5f), new Vector3(0, -0.5f, -0.5f), 2f);
        AudioController.PlaySound(dashSound);

        float time = 0;
        int beats = level < 7 ? 2 : 3;
        float speed = 1;
        float beatTime = 0;
        if (BeatManager.compassless) beatTime = BeatManager.GetBeatDuration();
        while (time < BeatManager.GetBeatDuration() / 3)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            if (BeatManager.isGameBeat && !BeatManager.compassless) beats--;
            if (BeatManager.compassless)
            {
                if (beatTime <= 0)
                {
                    beats--;
                    beatTime = BeatManager.GetBeatDuration();
                }
                else
                {
                    beatTime -= Time.deltaTime;
                }
            }

            if (beats == 0 && time == 0) spriteTrail.Stop();
            if (beats == 0) time += Time.deltaTime;

            Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
            Vector2 difference = (crosshairPos - (Vector2)transform.position).normalized;

            Vector2 dashDir = difference;
            if (dashDir.x > 0) facingRight = true;
            else facingRight = false;
            if (beats <= 1)
            {
                speed = Mathf.MoveTowards(speed, 0, Time.deltaTime / (BeatManager.GetBeatDuration() * 2));
                rb.velocity = dashDir * speed * currentStats.Speed * dashSpeed;
            } 
            else rb.velocity = dashDir * currentStats.Speed * dashSpeed;
            
            yield return new WaitForEndOfFrame();
        }
        rb.velocity = Vector2.zero;
        
        dashHitBox.enabled = false;
        //animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        animator.speed = 1f / BeatManager.GetBeatDuration();
        animator.Play("Rabi_Idle");
        Sprite.transform.localPosition = Vector3.zero;

        isPerformingAction = false;
        isCastingBunnyHop = false;
        isMoving = false;

        yield break;
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
        StopCoroutine(MoveCoroutine(direction));
        Vector2 dir = Vector2.zero;
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
        {
            dir.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            dir.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
        }
        else
        {
            Vector2 leftStick = InputManager.GetLeftStick();
            dir = leftStick.normalized;
        }
        if (dir == Vector2.zero) dir = facingRight ? Vector2.right : Vector2.left;
        dir.Normalize();
        dir *= currentStats.Speed * 2;
        /*
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)transform.position);

        Vector2 finalPos = crosshairPos;
        if (difference.magnitude > 3) finalPos = transform.position + (Vector3)(difference.normalized * 3);
        */
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
            
        //animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 2f); // / 2
        animator.speed = 1f / BeatManager.GetBeatDuration();
        animator.Play("Rabi_BunnyHop");
        spriteTrail.Play(Sprite, 10, 0.05f, Sprite.transform, new Color(0, 1, 1, 1f), new Vector3(0, 0, -0.5f), 1f);
        AudioController.PlaySound(dashSound);
        while (time <= BeatManager.GetBeatDuration())
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            if (Map.isWallAt(targetPos)) targetPos = originPos;
            // BeatDuration is 1
            // time is x

            /*
            if (time >= BeatManager.GetBeatDuration() / 3f && !hasSpawnedClone)
            {
                hasSpawnedClone = true;
                SmokeExplosion smokeExplosion = PoolManager.Get<SmokeExplosion>();
                smokeExplosion.transform.position = originPos;

                RabiClone rabiClone = PoolManager.Get<RabiClone>();
                rabiClone.transform.position = originPos;
                rabiClone.OnInit();
                playerClones.Add(rabiClone.gameObject);

            }*/
            float lerpedvalue = Mathf.Lerp(0,1f, time / BeatManager.GetBeatDuration());
            transform.position = Vector3.Lerp(originPos, (Vector3)targetPos, lerpedvalue);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        spriteTrail.Stop();
        transform.position = targetPos;
        //animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        animator.speed = 1f / BeatManager.GetBeatDuration();
        animator.Play("Rabi_Idle");
        Sprite.transform.localPosition = Vector3.zero;
        transform.position = targetPos;

        isPerformingAction = false;
        isCastingBunnyHop = false;
        isMoving = false;
        yield break;
    }

    public override void OnPassiveAbilityUse()
    {
        PlayerAbility a = equippedPassiveAbilities[currentWeapon];
        if (a.CanCast()) a.OnCast();
        /*
        foreach (PlayerAbility ability in equippedPassiveAbilities)
        {
            if (ability.CanCast()) ability.OnCast();
        }
        */
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

        if (invulTime > 0) return;
        else
        {
            invulTime = 0.2f;
        }

        bool doDamage = true;
        foreach (PlayerAbility ability in equippedPassiveAbilities)
        {
            if (ability.onPlayerPreHurt(damage) == false)
            {
                doDamage = false;
                break;
            }
        }

        if (!doDamage)
        {
            UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, 0, DamageTextType.Dodge);
            return;
        }

        CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, 9999);
        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.DoHurtEffect();
        if (hurtSfxCD <= 0)
        {
            PlayerCamera.TriggerCameraShake(0.2f, 0.1f);
            AudioController.PlaySound(AudioController.instance.sounds.playerHurtSfx);
            hurtSfxCD = 0.6f;
        }

        UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, damage, DamageTextType.PlayerDamage);

        emissionColor = new Color(1, 0, 0, 1);

        if (CurrentHP <= 0)
        {
            Die();
        }
    }
}
