using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;
    public static Player instance;
    public bool isPerformingAction;
    public PlayerCamera cam;

    public Sprite icon;
    public string charactername = "SHIKAMI RABI";
    public Animator animator;

    protected Vector2 direction, oldDir;
    public bool isMoving, canDoAnything;
    public bool facingRight;

    [SerializeField] public SpriteRenderer Sprite;
    public float SpriteSize = 1f;
    protected float SpriteX = 1f;

    public static Vector2 originPos;

    public bool onbeat;
    public bool waitForNextBeat;

    protected PlayerAction action;

    public int CurrentHP;
    public int MaxSP, CurrentSP;
    public int MaxExp, CurrentExp, Level;

    public int baseAtk = 12;

    protected Material spriteRendererMat;
    protected Color emissionColor = new Color(1,0,0,0);

    protected bool canUseActiveAbility = true;
    protected bool canUseUltimate = true;
    public bool isDead = false;
    protected float hurtSfxCD;

    [Header("Stats")]
    public PlayerStats baseStats;
    public PlayerStats perLevelStats;
    public PlayerStats currentStats;

    public PlayerStats flatBonusStats;
    public PlayerStats percentBonusStats;
    public Dictionary<string, float> abilityValues;
    public Dictionary<string, float> itemValues;

    public AudioClip music_theme;

    [SerializeReference] public List<Enhancement> enhancements;

    [Header("Abilities")]
    public List<PlayerAbility> equippedPassiveAbilities;
    public PlayerAbility activeAbility;
    public PlayerAbility ultimateAbility;

    [Header("Items")]
    public List<PlayerItem> equippedItems;
    public List<PlayerItem> evolvedItems;

    public List<GameObject> playerClones;
    public bool isInvulnerable;
    [SerializeField] public SpriteRenderer grazeSprite;

    [SerializeField] LayerMask transparencyLayerMask;

    public float invulTime;

    public GameObject Exclamation;
    public int poisonStatus;
    public List<IDespawneable> despawneables;

    public int currentWeapon;
    public bool isShooting;
    [SerializeField] public SpriteRenderer rechargeFX;
    public Color rechargeFXcolor;

    public bool isPoisoned()
    {
        return poisonStatus > 0;
    }

    public int GetPoisonDamage()
    {
        return poisonStatus;
    }

    public void SetMaxSP(int n)
    {
        bool playSound = false;
        if (CurrentSP < MaxSP) playSound = true;
        MaxSP = n;
        if (CurrentSP > MaxSP)
        {
            CurrentSP = MaxSP;
        }
        if (CurrentSP == MaxSP && playSound)
        {
             AudioController.PlaySound(AudioController.instance.sounds.playerSpecialAvailableSfx);
        }
        instance.CurrentSP = Mathf.Clamp(instance.CurrentSP + n, 0, MaxSP);
        UIManager.Instance.PlayerUI.UpdateSpecial();

    }

    public void ForceDespawnAbilities(bool instant)
    {
        foreach (IDespawneable despawneable in despawneables)
        {
            despawneable.ForceDespawn(instant);
        }
        despawneables.Clear();
    }

    public virtual void ResetAbilities()
    {
        foreach (PlayerAbility ability in equippedPassiveAbilities)
        {
            ability.currentCooldown = 1;
        }
    }

    public Vector3 GetClosestPlayer(Vector2 pos)
    {
        if (playerClones.Count == 0) return transform.position;

        Vector3 finalPos = playerClones[0].transform.position;
        float dist = Vector2.Distance(pos, finalPos);
        foreach (var playerClone in playerClones)
        {
            float newdist = Vector2.Distance(pos, playerClone.transform.position);
            if (newdist < dist)
            {
                dist = newdist;
                finalPos = playerClone.transform.position;
            }
        }
        return finalPos;
    }

    public virtual List<Enhancement> GetAttackEnhancementList() { return null; }

    public bool CanMove()
    {
        if (isPerformingAction) return false;
        else return BeatManager.GetBeatSuccess() != BeatTrigger.FAIL;
    }

    public int getPassiveAbilityIndex(System.Type abilityType)
    {
        PlayerAbility searchAbility = equippedPassiveAbilities.FirstOrDefault<PlayerAbility>(x=> x.GetType() ==abilityType);
        if (searchAbility != null)
        {
            return equippedPassiveAbilities.IndexOf(searchAbility);
        }
        else
        {
            return equippedPassiveAbilities.Count();
        }
    }

    public int getItemIndex(System.Type itemType)
    {
        PlayerItem searchAbility = equippedItems.FirstOrDefault<PlayerItem>(x => x.GetType() == itemType);
        if (searchAbility != null)
        {
            return equippedItems.IndexOf(searchAbility);
        }
        else
        {
            return equippedItems.Count();
        }
    }

    public virtual void Despawn()
    {

    }

    public static void ResetPosition()
    {
        instance.transform.position = Vector3.zero;
        instance.cam.SetOnPlayer();
        instance.animator.speed = 1 / BeatManager.GetBeatDuration();
        instance.animator.updateMode = AnimatorUpdateMode.Normal;
        instance.Sprite.sortingLayerID = 0;
    }
    protected virtual void Awake()
    {
        instance = this;
        cam = PlayerCamera.instance;
        UIManager.Instance.PlayerUI.SetPlayerCharacter(icon, charactername);
        abilityValues = new Dictionary<string, float>();
        itemValues = new Dictionary<string, float>();
        despawneables = new List<IDespawneable>();
        equippedPassiveAbilities = new List<PlayerAbility>();
        equippedItems = new List<PlayerItem>();
        evolvedItems = new List<PlayerItem>();
        spriteRendererMat = Sprite.material;
        Exclamation.SetActive(false);
        Level = 1;
        MaxExp = CalculateExpCurve(Level);

        CalculateStats();
        CurrentHP = (int)currentStats.MaxHP;
        CurrentSP = 0;

        grazeSprite.color = Color.clear;

        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.UpdateExp();
        UIManager.Instance.PlayerUI.UpdateSpecial();
        UIManager.Instance.PlayerUI.coinText.text = "0";

        itemValues.Add("orbitalSpeed", 1.0f);
        itemValues.Add("orbitalDamage", 1.0f);
        itemValues.Add("explosionDamage", 1.0f);
        itemValues.Add("explosionSize", 1.0f);
        itemValues.Add("burnDamage", 1.0f);
        itemValues.Add("burnDuration", 3);
    }

    public int CalculateExpCurve(int lv)
    {
        if (lv == 1) return 80;

        float a = Mathf.Pow(4 * (lv + 1), 2.1f);
        float b = Mathf.Pow(4 * lv + 1, 2.1f);
        int exp = (int)(Mathf.Round(a) - Mathf.Round(b));

        return exp;

        //Mathf.Round((4*(lv + 1)))
        //int nextLevel = (int)Mathf.Pow(lv, 2);
        //int lastLevel = (int)Mathf.Pow(lv - 1, 2);
        //return (nextLevel - lastLevel) + 50;
    }

    public bool hasEvolvableAbility()
    {
        foreach (PlayerAbility ability in equippedPassiveAbilities)
        {
            if (ability.isEvolved()) continue;

            PlayerItem item = equippedItems.FirstOrDefault(x => x.GetType() == ability.getEvolutionItemType());
            if (item == null) continue;
            return true;
        }
        return false;
    }
    
    public void CalculateStats()
    {
        currentStats = baseStats.Copy();
        currentStats += perLevelStats * Level;
        
        flatBonusStats = new PlayerStats();
        percentBonusStats = new PlayerStats();

        foreach (Enhancement enhancement in enhancements)
        {
            enhancement.OnStatCalculate(ref flatBonusStats, ref percentBonusStats);
        }

        currentStats += flatBonusStats;
        currentStats *= percentBonusStats;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -60f);
        facingRight = true;
        canDoAnything = true;
        cam.SetOnPlayer();
        cam.followPlayer = true;
    }

    public void Heal(float amount)
    {
        float baseamount = amount;
        foreach (PlayerItem item in equippedItems)
        {
            amount += item.OnPreHeal(baseamount) - baseamount;
        }
        foreach (PlayerItem item in evolvedItems)
        {
            amount += item.OnPreHeal(baseamount) - baseamount;
        }
        if (amount == 0) return;

        bool canCrit = false;
        if (itemValues.ContainsKey("item.blessedfigure.level")) canCrit = true;
        bool isCritical = false;
        if (canCrit)
        {
           isCritical = currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) amount *= currentStats.CritDmg;
        }
        
        amount = Mathf.RoundToInt(amount);
        CurrentHP = (int)Mathf.Clamp(CurrentHP + amount, 0, currentStats.MaxHP);
        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, (int)amount, isCritical ? DamageTextType.CriticalHeal : DamageTextType.Heal);
        AudioController.PlaySound(AudioController.instance.sounds.healSound);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isLoading || GameManager.isPaused || !canDoAnything)
        {
            UIManager.Instance.PlayerUI.activeCDImage.fillAmount = 0;
            activeAbility.currentCooldown = 0;
            UIManager.Instance.PlayerUI.activeCDImage.transform.position = Sprite.transform.position + new Vector3(0.7f, 0.9f, 1f);
            HandleSprite();
            isShooting = false;
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame && equippedPassiveAbilities.Count > 0) SetEquippedAbility(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame && equippedPassiveAbilities.Count > 1) SetEquippedAbility(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame && equippedPassiveAbilities.Count > 2) SetEquippedAbility(2);

        invulTime = Mathf.MoveTowards(invulTime, 0, Time.deltaTime);

        if (!isDead)
        {
            HandleInput();
        }
        /*
        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            Player.AddSP(250);
            UIManager.Instance.PlayerUI.UpdateSpecial();
        }*/
        
        HandleSprite();
        if (hurtSfxCD > 0) hurtSfxCD -= Time.deltaTime;

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            Level++;
            OnLevelUp();
        }

        if (BeatManager.isGameBeat)
        {
            if (ultimateAbility != null) AddSP(1);

            UIManager.Instance.PlayerUI.UpdateSpecial();
            animator.updateMode = AnimatorUpdateMode.Normal;
            if (poisonStatus > 5) poisonStatus = 5;
            if (poisonStatus > 0) poisonStatus--;
            if (poisonStatus > 0) TakeDamage(GetPoisonDamage());
        }

        if (equippedPassiveAbilities[currentWeapon].currentAmmo == 0)
        {
            rechargeFXcolor.r = equippedPassiveAbilities[currentWeapon].GetRechargeColor().r;
            rechargeFXcolor.g = equippedPassiveAbilities[currentWeapon].GetRechargeColor().g;
            rechargeFXcolor.b = equippedPassiveAbilities[currentWeapon].GetRechargeColor().b;
            rechargeFXcolor.a = Mathf.Lerp(rechargeFXcolor.a, 1, Time.deltaTime * 8f);
            rechargeFX.color = rechargeFXcolor;
        }
        else
        {
            rechargeFXcolor.a = Mathf.Lerp(rechargeFXcolor.a, 0, Time.deltaTime * 8f);
            rechargeFX.color = rechargeFXcolor;
        }

        foreach (PlayerAbility ability in equippedPassiveAbilities)
        {
            if (ability != null) ability.OnUpdate();
        }
        foreach (PlayerItem item in equippedItems)
        {
            if (item != null) item.OnUpdate();
        }
        foreach (PlayerItem item in evolvedItems)
        {
            if (item != null) item.OnUpdate();
        }

        if (activeAbility != null) { activeAbility.OnUpdate(); }
        if (ultimateAbility != null) { ultimateAbility.OnUpdate(); }

        if (activeAbility == null) UIManager.Instance.PlayerUI.activeCDImage.fillAmount = 0;
        else
        {
            float currentCD = activeAbility.currentCooldown;
            float maxCD = activeAbility.maxCooldown;

            if (currentCD == 0) UIManager.Instance.PlayerUI.activeCDImage.fillAmount = 0;
            else UIManager.Instance.PlayerUI.activeCDImage.fillAmount = (currentCD / maxCD);

        }
        UIManager.Instance.PlayerUI.activeCDImage.transform.position = Sprite.transform.position + new Vector3(0.7f, 0.9f, 1f);

    }

    public void SetEquippedAbility(int n)
    {
        if (currentWeapon == n) return;
        equippedPassiveAbilities[currentWeapon].OnChange();
        currentWeapon = n;
        equippedPassiveAbilities[currentWeapon].OnSelect();
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    public virtual void OnPassiveAbilityUse() { }

    public virtual void OnActiveAbilityUse() { }

    public virtual void OnUltimateUse() { }

    void HandleInput()
    {
        if (BeatManager.compassless)
        {
            HandleCompasslessInput();
            return;
        }
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
        {
            direction.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            direction.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
        }
        else
        {
            Vector2 leftStick = InputManager.GetLeftStick();
            direction.x = leftStick.x > 0.4f ? 1 : leftStick.x < -0.4f ? -1 : 0;
            direction.y = leftStick.y > 0.4f ? 1 : leftStick.y < -0.4f ? -1 : 0;
        }
        
        if (direction !=  Vector2.zero) {
            oldDir = direction;
        }

        if (InputManager.IsStickMovementThisFrame())
        {
            action = PlayerAction.Move;
        }
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            action = PlayerAction.Move;
        }

        if (BeatManager.isQuarterBeat && InputManager.ActionHold(InputActionType.ATTACK))
        {
            PerformAutomatedAction();
        }

        if (InputManager.ActionPress(InputActionType.ABILITY))
        {
            if (activeAbility != null)
            {
                BeatTrigger result = BeatManager.GetBeatSuccess();
                if (result != BeatTrigger.FAIL && activeAbility.CanCast())
                {
                    OnActiveAbilityUse();
                }
                else
                {
                    PlayerCamera.TriggerCameraShake(0.04f, 0.2f);
                    BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                    
                    //waitForNextBeat = true;
                }
            }

        }

        if (InputManager.ActionPress(InputActionType.ULTIMATE))
        {
            if (ultimateAbility != null)
            {
                BeatTrigger result = BeatManager.GetBeatSuccess();
                if (result != BeatTrigger.FAIL && !waitForNextBeat && ultimateAbility.CanCast())
                {
                    OnUltimateUse();
                }
                else
                {
                    PlayerCamera.TriggerCameraShake(0.04f, 0.2f);
                    BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                    //waitForNextBeat = true;
                }
            }
        }

        // Handle Movement
        if (BeatManager.compassless && action != PlayerAction.None)
        {
            PerformAction(action);
        }
        else
        {
            if (!waitForNextBeat && !isPerformingAction)
            {
                BeatTrigger score = BeatManager.GetBeatSuccess();
                if (action != PlayerAction.None)
                {
                    // Too late to the beat = fail
                    if (!BeatManager.closestIsNextBeat() && score == BeatTrigger.FAIL)
                    {
                        PlayerCamera.TriggerCameraShake(0.04f, 0.2f);
                        BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                        action = PlayerAction.None;
                    }
                    else if (BeatManager.closestIsNextBeat() && score == BeatTrigger.FAIL)
                    {
                        PlayerCamera.TriggerCameraShake(0.04f, 0.2f);
                        BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                        waitForNextBeat = true;
                        action = PlayerAction.None;
                    }
                    else if (!isPerformingAction)
                    {
                        if (BeatManager.GetBeatSuccess() == BeatTrigger.SUCCESS || score == BeatTrigger.PERFECT)
                        {
                            BeatManager.TriggerBeatScore(score);
                            PerformAction(action);
                            waitForNextBeat = true;
                        }
                    }
                }
            }
        
        }
        if (BeatManager.compassless)
        {
            waitForNextBeat = false;
        }
        else
        {
            if (waitForNextBeat && !BeatManager.closestIsNextBeat() && BeatManager.GetBeatSuccess() == BeatTrigger.FAIL)
            {
                waitForNextBeat = false;
            }
        }
    }

    void HandleCompasslessInput()
    {
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
        {
            direction.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            direction.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
            action = PlayerAction.Move;
        }
        else
        {
            Vector2 leftStick = InputManager.GetLeftStick();
            direction.x = leftStick.x > 0.4f ? 1 : leftStick.x < -0.4f ? -1 : 0;
            direction.y = leftStick.y > 0.4f ? 1 : leftStick.y < -0.4f ? -1 : 0;
            action = PlayerAction.Move;
        }
        if (direction != Vector2.zero)
        {
            oldDir = direction;
        }
        if (direction == Vector2.zero)
        {
            action = PlayerAction.None;
        }
        else
        {
            Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
            Vector2 difference = (crosshairPos - (Vector2)transform.position).normalized;

            Vector2 lookDir = difference;
            if (difference.x < 0) facingRight = false;
            if (difference.x > 0) facingRight = true;
        }

        if (InputManager.ActionHold(InputActionType.ATTACK)) isShooting = true;
        else isShooting = false;
        /*
        if (isShooting)
        {
            facingRight = direction.x > 0;
        }*/

        if (BeatManager.isQuarterBeat && InputManager.ActionHold(InputActionType.ATTACK))
        {
            PerformAutomatedAction();
        }

        if (InputManager.ActionPress(InputActionType.ABILITY))
        {
            if (activeAbility != null)
            {
                if (activeAbility.CanCast())
                {
                    OnActiveAbilityUse();
                }
            }

        }

        if (InputManager.ActionPress(InputActionType.ULTIMATE))
        {
            if (ultimateAbility != null)
            {
                if (ultimateAbility.CanCast())
                {
                    OnUltimateUse();
                }
            }
        }

        // Handle Movement
        if (action != PlayerAction.None && BeatManager.isGameBeat)
        {
            PerformAction(action);
        }
        waitForNextBeat = false;
    }

    public void PerformAutomatedAction()
    {
        if (direction.x < 0) facingRight = false;
        else facingRight = true;
        OnPassiveAbilityUse();
    }

    private void PerformAction(PlayerAction Playeraction)
    {
        if (isMoving) return;
        if (isPerformingAction) return;
        action = PlayerAction.None;
        isPerformingAction = true;
        switch (Playeraction)
        {
            case PlayerAction.Move:
                Move((Vector2)transform.position + direction);
                break;
        }
        //if (!BeatManager.compassless) PerformAutomatedAction();

        BeatManager.OnPlayerAction();
    }

    public virtual void Move(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
    }

    protected virtual IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        SpriteSize = 1.2f;
        isMoving = true;
        Vector3 originalPos = transform.position;
        float height = 0;
        float time = 0;

        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
        {
            direction.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            direction.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
        }
        else
        {
            Vector2 leftStick = InputManager.GetLeftStick();
            direction.x = leftStick.x > 0.4f ? 1 : leftStick.x < -0.4f ? -1 : 0;
            direction.y = leftStick.y > 0.4f ? 1 : leftStick.y < -0.4f ? -1 : 0;
        }
        if (direction != Vector2.zero)
        {
            oldDir = direction;
        }


        if (direction == Vector2.zero)
        {
            direction = oldDir;
            targetPos = (Vector2)originalPos + oldDir;
        }
        if (Map.isWallAt(targetPos)) targetPos = originalPos;

        while (time <= 0.125f)
        {
            if (Map.isWallAt(targetPos)) targetPos = originalPos;

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

        isPerformingAction = false;
        isMoving = false;
        yield break;
    }

    


    protected void HandleSprite()
    {
        SpriteSize = Mathf.MoveTowards(SpriteSize, 1f, Time.deltaTime * 4f);
        SpriteX = Mathf.MoveTowards(SpriteX, facingRight ? 1 : -1, Time.deltaTime * 24f);
        Sprite.transform.localScale = new Vector3(SpriteX, 1, 1) * SpriteSize;
        
        emissionColor = Color.Lerp(emissionColor, new Color(1, 0, 0, 0), Time.deltaTime * 16f);
        spriteRendererMat.SetColor("_EmissionColor", emissionColor);
    }

   

    public virtual void TakeDamage(int damage)
    {
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
       

        emissionColor = new Color(1, 0, 0, 1);

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public static void AddExp(int n)
    {
        n = (int)(n * instance.currentStats.ExpMulti);
        if (instance.isDead) return;
        instance.CurrentExp += n;
        if (instance.CurrentExp >= instance.MaxExp)
        {
            instance.CurrentExp -= instance.MaxExp;
            instance.Level++;
            instance.OnLevelUp();
        }
        UIManager.Instance.PlayerUI.UpdateExp();
    }

    public static void AddSP(int n)
    {
        if (instance.isDead) return;
        if (instance.CurrentSP < instance.MaxSP && instance.CurrentSP + n >= instance.MaxSP) AudioController.PlaySound(AudioController.instance.sounds.playerSpecialAvailableSfx);
        instance.CurrentSP = Mathf.Clamp(instance.CurrentSP + n, 0, instance.MaxSP);
        UIManager.Instance.PlayerUI.UpdateSpecial();
    }

    public void OnLevelUp()
    {
        MaxExp = CalculateExpCurve(Level);
        AudioController.PlaySound(AudioController.instance.sounds.playerLvlUpSfx);
        CalculateStats();
        StartCoroutine(OpenEnhancementMenuCoroutine());
    }

    protected IEnumerator OpenEnhancementMenuCoroutine()
    {
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();
        BeatManager.isGameBeat = false;
        EnhancementMenu.instance.Open();
    }

    public void OpenEvolutionMenu()
    {
        StartCoroutine(OpenEvolutionMenuCoroutine());
    }
    protected IEnumerator OpenEvolutionMenuCoroutine()
    {
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();
        BeatManager.isGameBeat = false;
        EvolutionUI.instance.Open();
    }

    public void Die()
    {
        isDead = true;
        StopAllCoroutines();
        cam.ResetOffset();
        cam.SetOnPlayer();
        emissionColor = Color.clear;
        spriteRendererMat.SetColor("_EmissionColor", emissionColor);

        StartCoroutine(DeathCoroutine());
    }

    protected virtual IEnumerator DeathCoroutine()
    {
        BeatManager.Stop();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Sprite.sortingLayerName = "UI";
        animator.Play("PlayerDev_Dead");

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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        HideableObject h = collision.GetComponent<HideableObject>();
        if (h != null) h.Hide();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        HideableObject h = collision.GetComponent<HideableObject>();
        if (h != null) h.UnHide();
    }
}
