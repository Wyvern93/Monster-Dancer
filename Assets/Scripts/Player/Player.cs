using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;
    public bool isPerformingAction;

    public Animator animator;

    protected Vector2 direction, oldDir;
    public bool isMoving;
    public bool facingRight { get; protected set; }

    [SerializeField] protected SpriteRenderer Sprite;
    public float SpriteSize = 1f;
    protected float SpriteX = 1f;

    public static Vector2 originPos;

    public bool onbeat;
    public bool waitForNextBeat;

    protected float ScreenShakeStrength;
    protected float ScreenShakeTime;

    private Vector3 targetCameraPos;
    protected Vector2 ScreenShakeDir;
    protected Vector3 CameraOffset;
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

    [Header("Stats")]
    public PlayerStats baseStats;
    public PlayerStats perLevelStats;
    public PlayerStats currentStats;

    public PlayerStats flatBonusStats;
    public PlayerStats percentBonusStats;
    public Dictionary<string, float> abilityValues;

    public AudioClip music_theme;

    [SerializeReference] public List<Enhancement> enhancements;

    [Header("Abilities")]
    public List<PlayerAbility> equippedPassiveAbilities;
    public PlayerAbility activeAbility;
    public PlayerAbility ultimateAbility;

    [Header("Items")]
    public List<PlayerItem> playerItems;

    [Header("Prefabs")]
    public GameObject attackPrefab;

    public List<GameObject> playerClones;
    public bool isInvulnerable;
    [SerializeField] public SpriteRenderer grazeSprite;


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

    public virtual void Despawn()
    {

    }

    public static void ResetPosition()
    {
        instance.transform.position = Vector3.zero;
        instance.targetCameraPos = new Vector3(instance.transform.position.x, instance.transform.position.y, -60);
        Camera.main.transform.position = instance.targetCameraPos;
        instance.animator.speed = 1 / BeatManager.GetBeatDuration();
        instance.animator.updateMode = AnimatorUpdateMode.Normal;
        instance.Sprite.sortingLayerID = 0;
    }
    protected virtual void Awake()
    {
        instance = this;
        abilityValues = new Dictionary<string, float>();
        equippedPassiveAbilities = new List<PlayerAbility>();
        playerItems = new List<PlayerItem>();
        spriteRendererMat = Sprite.material;
        Level = 1;
        MaxExp = CalculateExpCurve(Level);

        CalculateStats();
        CurrentHP = (int)currentStats.MaxHP;
        CurrentSP = 0;

        grazeSprite.color = Color.clear;

        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.UpdateExp();
        UIManager.Instance.PlayerUI.UpdateSpecial();
    }

    public int CalculateExpCurve(int lv)
    {
        int nextLevel = (int)Mathf.Pow(lv, 2);
        int lastLevel = (int)Mathf.Pow(lv - 1, 2);
        return (nextLevel - lastLevel) + 50;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isLoading) return;
        if (GameManager.isPaused) return;
        if (!isDead)
        {
            HandleInput();
        }
        
        HandleSprite();
        HandleCamera();

        if (BeatManager.isGameBeat)
        {
            CurrentSP = Mathf.Clamp(CurrentSP + 1, 0, MaxSP);
            UIManager.Instance.PlayerUI.UpdateSpecial();
            animator.speed = 1 / BeatManager.GetBeatDuration();
            animator.updateMode = AnimatorUpdateMode.Normal;
        }

        foreach (PlayerAbility ability in equippedPassiveAbilities)
        {
            if (ability != null) ability.OnUpdate();
        }
        if (activeAbility != null) { activeAbility.OnUpdate(); }
        if (ultimateAbility != null) { ultimateAbility.OnUpdate();}

        if (activeAbility == null) UIManager.Instance.PlayerUI.activeCDImage.fillAmount = 0;
        else
        {
            float currentCD = activeAbility.currentCooldown;
            float maxCD = activeAbility.maxCooldown;

            UIManager.Instance.PlayerUI.activeCDImage.fillAmount = (currentCD / maxCD);
        }
        UIManager.Instance.PlayerUI.activeCDImage.transform.position = Sprite.transform.position + new Vector3(0.7f, 0.9f, 1f);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    public virtual void OnAttack()
    {
        PlayerAttack atkEntity = PoolManager.Get<PlayerAttack>();
        atkEntity.Attack(direction);
        if (direction.x < 0) facingRight = false;
        else facingRight = true;
    }

    public virtual void OnPassiveAbility1Use() { }

    public virtual void OnPassiveAbility2Use() { }

    public virtual void OnPassiveAbility3Use() { }

    public virtual void OnActiveAbilityUse() { }

    public virtual void OnUltimateUse() { }

    void HandleInput()
    {
        //direction = new Vector2(Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0, Keyboard.current.sKey.isPressed ? -1 : Keyboard.current.wKey.isPressed ? 1 : 0);
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
            if (direction.x == -1) facingRight = false;
            if (direction.x == 1) facingRight = true;
        }
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            action = PlayerAction.Move;
            if (direction.x == -1) facingRight = false;
            if (direction.x == 1) facingRight = true;
        }

        if (BeatManager.isGameBeat && !GameManager.compassless) 
        {
            PerformAutomatedAction();


            /*
            BeatTrigger result = BeatManager.GetBeatSuccess();
            if (result != BeatTrigger.FAIL && !waitForNextBeat && canAttack)
            {
                OnAttack();
                canAttack = false;
            }
            else
            {
                TriggerCameraShake(0.04f, 0.2f);
                BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                waitForNextBeat = true;
                canAttack = false;
            }*/
        }

        if (InputManager.ActionPress(InputActionType.ABILITY))
        {
            if (activeAbility != null)
            {
                BeatTrigger result = BeatManager.GetBeatSuccess();
                if (result != BeatTrigger.FAIL && !waitForNextBeat && activeAbility.CanCast())
                {
                    OnActiveAbilityUse();
                }
                else
                {
                    TriggerCameraShake(0.04f, 0.2f);
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
                    TriggerCameraShake(0.04f, 0.2f);
                    BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                    //waitForNextBeat = true;
                }
            }
        }

        // Handle Movement
        if (GameManager.compassless && action != PlayerAction.None)
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
                        TriggerCameraShake(0.04f, 0.2f);
                        BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                        action = PlayerAction.None;
                    }
                    else if (BeatManager.closestIsNextBeat() && score == BeatTrigger.FAIL)
                    {
                        TriggerCameraShake(0.04f, 0.2f);
                        BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                        waitForNextBeat = true;
                        action = PlayerAction.None;
                    }
                    else if (!isPerformingAction)
                    {
                        if (BeatManager.GetBeatSuccess() == BeatTrigger.SUCCESS || score == BeatTrigger.PERFECT)
                        {
                            if (BeatManager.isBeatAfter() || BeatManager.isBeat)
                            {
                                BeatManager.TriggerBeatScore(score);
                                PerformAction(action);
                                waitForNextBeat = true;
                            }
                        }
                    }
                }
            }
        
        }
        if (GameManager.compassless)
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
        
        /*
        if (!BeatManager.closestIsNextBeat() && BeatManager.GetBeatSuccess() == BeatTrigger.FAIL)
        {
            if (!canAttack) canAttack = true;
            if (!canUseAbility1) canUseAbility1 = true;
            if (!canUseAbility2) canUseAbility2 = true;
            if (!canUseAbility3) canUseAbility3 = true;
            if (!canUseUltimate) canUseUltimate = true;
        }*/
    }

    public void PerformAutomatedAction()
    {
        OnAttack();
        if (equippedPassiveAbilities.Count > 0) OnPassiveAbility1Use();
        if (equippedPassiveAbilities.Count > 1) OnPassiveAbility2Use();
        if (equippedPassiveAbilities.Count > 2) OnPassiveAbility3Use();
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
        PerformAutomatedAction();
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

    public static void TriggerCameraShake(float strength, float time)
    {
        if (strength > instance.ScreenShakeStrength) instance.ScreenShakeStrength = strength;
        if (time > instance.ScreenShakeTime) instance.ScreenShakeTime = time;
    }

    private IEnumerator CameraShakeCoroutine()
    {
        float timeExponential = 1 / ScreenShakeTime;
        while (ScreenShakeTime > 0f)
        {
            while (Time.timeScale <= 0.01f)
            {
                yield return new WaitForEndOfFrame();
            }
            ScreenShakeTime -= Time.deltaTime;
            CameraOffset = (Vector3)Random.insideUnitCircle * ScreenShakeStrength;
            ScreenShakeStrength = Mathf.MoveTowards(ScreenShakeTime, 0, timeExponential * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        CameraOffset = Vector3.zero;
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

    protected void HandleCamera()
    {
        targetCameraPos = Vector3.Lerp(targetCameraPos, new Vector3(transform.position.x, transform.position.y, -10), Time.deltaTime * 8f);

        // Camera ScreenShake
        if (ScreenShakeTime > 0)
        {
            ScreenShakeTime = Mathf.MoveTowards(ScreenShakeTime, 0, Time.deltaTime);
            float currentShakeStrength = ScreenShakeStrength * ScreenShakeTime;
            ScreenShakeDir = Random.insideUnitCircle * currentShakeStrength;
        }
        CameraOffset = Vector3.MoveTowards(CameraOffset, ScreenShakeDir, Time.deltaTime * 24f);

        Camera.main.transform.position = targetCameraPos + CameraOffset;
        Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -10, 9.5f), Mathf.Clamp(Camera.main.transform.position.y, -6.88f, 6.25f), Camera.main.transform.position.z);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isInvulnerable) return;
        if (isDead) return;
        CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, 9999);
        TriggerCameraShake(0.3f, 0.2f);
        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.DoHurtEffect();
        AudioController.PlaySound(AudioController.instance.sounds.playerHurtSfx);

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
        instance.CurrentSP = Mathf.Clamp(instance.CurrentSP + n, 0, 250);
        UIManager.Instance.PlayerUI.UpdateSpecial();
    }

    public void OnLevelUp()
    {
        MaxExp = CalculateExpCurve(Level);
        AudioController.PlaySound(AudioController.instance.sounds.playerLvlUpSfx);
        CalculateStats();
        EnhancementMenu.instance.Open();
    }

    public void Die()
    {
        isDead = true;
        StopAllCoroutines();
        CameraOffset = Vector3.zero;
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -60);
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
}
