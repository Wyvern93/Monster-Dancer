using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;
    public bool isPerformingAction;

    public Animator animator;

    protected Vector2 direction, oldDir;
    public bool facingRight { get; protected set; }

    [SerializeField] protected SpriteRenderer Sprite;
    public float SpriteSize = 1f;
    protected float SpriteX = 1f;

    public static Vector2 position {  get; protected set; }

    public bool onbeat;
    public bool waitForNextBeat;

    protected float ScreenShakeStrength;
    protected float ScreenShakeTime;
    protected Vector2 ScreenShakeDir;
    protected Vector3 CameraOffset;
    protected PlayerAction action;

    public int CurrentHP;
    public int MaxSP, CurrentSP;
    public int MaxExp, CurrentExp, Level;

    public int baseAtk = 12;

    protected Material spriteRendererMat;
    protected Color emissionColor = new Color(1,0,0,0);

    bool canAttack = true;
    bool canUseAbility1 = true;
    bool canUseAbility2 = true;
    bool canUseAbility3 = true;
    bool canUseUltimate = true;
    bool isDead = false;

    [Header("Stats")]
    public PlayerStats baseStats;
    public PlayerStats perLevelStats;
    public PlayerStats currentStats;

    public PlayerStats flatBonusStats;
    public PlayerStats percentBonusStats;
    public Dictionary<string, float> abilityValues;

    [SerializeReference] public List<Enhancement> enhancements;

    [Header("Abilities")]
    public List<PlayerAbility> playerAbilities;
    public PlayerAbility[] equippedAbilities;

    [Header("Prefabs")]
    public GameObject attackPrefab;

    public bool CanMove()
    {
        if (isPerformingAction) return false;
        else return BeatManager.GetBeatSuccess() != BeatTrigger.FAIL;
    }

    public virtual void Despawn()
    {

    }

    public static void ResetPosition()
    {
        instance.transform.position = Vector3.zero;
        position = Vector3.zero;
        //BeatManager.SetRingPosition(Vector3.zero);
        Camera.main.transform.position = new Vector3(position.x, position.y, -10);
        instance.animator.speed = 1 / BeatManager.GetBeatDuration();
        instance.animator.updateMode = AnimatorUpdateMode.Normal;
        instance.Sprite.sortingLayerID = 0;
    }
    protected virtual void Awake()
    {
        instance = this;
        abilityValues = new Dictionary<string, float>();
        equippedAbilities = new PlayerAbility[4];
        spriteRendererMat = Sprite.material;
        Level = 1;
        MaxExp = Level ^ 3 + 50;

        enhancements.Add(new StatHPEnhancement());
        enhancements.Add(new StatHPEnhancement());
        CalculateStats();
        CurrentHP = currentStats.MaxHP;

        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.UpdateExp();
        UIManager.Instance.PlayerUI.UpdateSpecial();
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
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        facingRight = true;
        position = transform.position;
        //BeatManager.SetRingPosition(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //BeatManager.SetRingPosition(transform.position);
        if (GameManager.isLoading) return;
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

        foreach (PlayerAbility ability in equippedAbilities)
        {
            if (ability != null) ability.OnUpdate();
        }

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

    public virtual void OnAbility1Use() { }

    public virtual void OnAbility2Use() { }

    public virtual void OnAbility3Use() { }

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

        if (InputManager.ActionPress(InputActionType.ATTACK)) 
        {
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
            }
        }

        if (InputManager.ActionPress(InputActionType.ABILITY1))
        {
            if (equippedAbilities[0] != null)
            {
                BeatTrigger result = BeatManager.GetBeatSuccess();
                if (result != BeatTrigger.FAIL && !waitForNextBeat && canUseAbility1 && equippedAbilities[0].CanCast())
                {
                    OnAbility1Use();
                    canUseAbility1 = false;
                }
                else
                {
                    TriggerCameraShake(0.04f, 0.2f);
                    BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                    waitForNextBeat = true;
                    canUseAbility1 = false;
                }
            }
        }

        if (InputManager.ActionPress(InputActionType.ABILITY2))
        {
            if (equippedAbilities[1] != null)
            {
                BeatTrigger result = BeatManager.GetBeatSuccess();
                if (result != BeatTrigger.FAIL && !waitForNextBeat && canUseAbility2 && equippedAbilities[1].CanCast())
                {
                    OnAbility2Use();
                    canUseAbility2 = false;
                }
                else
                {
                    TriggerCameraShake(0.04f, 0.2f);
                    BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                    waitForNextBeat = true;
                    canUseAbility2 = false;
                }
            }
        }

        if (InputManager.ActionPress(InputActionType.ABILITY3))
        {
            if (equippedAbilities[2] != null)
            {
                BeatTrigger result = BeatManager.GetBeatSuccess();
                if (result != BeatTrigger.FAIL && !waitForNextBeat && canUseAbility3 && equippedAbilities[2].CanCast())
                {
                    OnAbility3Use();
                    canUseAbility3 = false;
                }
                else
                {
                    TriggerCameraShake(0.04f, 0.2f);
                    BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                    waitForNextBeat = true;
                    canUseAbility3 = false;
                }
            }
        }

        if (InputManager.ActionPress(InputActionType.ULTIMATE))
        {
            if (equippedAbilities[3] != null && CurrentSP == MaxSP)
            {
                BeatTrigger result = BeatManager.GetBeatSuccess();
                if (result != BeatTrigger.FAIL && !waitForNextBeat && canUseUltimate && equippedAbilities[3].CanCast())
                {
                    OnUltimateUse();
                    canUseUltimate = false;
                }
                else
                {
                    TriggerCameraShake(0.04f, 0.2f);
                    BeatManager.TriggerBeatScore(BeatTrigger.FAIL);
                    waitForNextBeat = true;
                    canUseUltimate = false;
                }
            }
        }

        // Handle Movement
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
        if (waitForNextBeat && !BeatManager.closestIsNextBeat() && BeatManager.GetBeatSuccess() == BeatTrigger.FAIL)
        {
            waitForNextBeat = false;
        }

        if (!BeatManager.closestIsNextBeat() && BeatManager.GetBeatSuccess() == BeatTrigger.FAIL)
        {
            if (!canAttack) canAttack = true;
            if (!canUseAbility1) canUseAbility1 = true;
            if (!canUseAbility2) canUseAbility2 = true;
            if (!canUseAbility3) canUseAbility3 = true;
            if (!canUseUltimate) canUseUltimate = true;
        }
    }

    private void PerformAction(PlayerAction Playeraction)
    {
        if (isPerformingAction) return;
        action = PlayerAction.None;
        isPerformingAction = true;
        switch (Playeraction)
        {
            case PlayerAction.Move:
                Move((Vector2)transform.position + direction);
                break;
        }
    }

    public void Move(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
    }

    protected virtual IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        SpriteSize = 1.2f;
        Vector3 originalPos = transform.position;
        float height = 0;
        float time = 0;
        if (direction == Vector2.zero)
        {
            direction = oldDir;
            targetPos = (Vector2)originalPos + oldDir;
        }
        if (Map.isWallAt(targetPos)) targetPos = originalPos;

        position = targetPos;
        BeatManager.OnPlayerAction();


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
        position = targetPos;

        isPerformingAction = false;
        yield break;
    }

    public static void TriggerCameraShake(float strength, float time)
    {
        instance.ScreenShakeStrength = strength;
        instance.ScreenShakeTime = time;
        instance.StartCoroutine(instance.CameraShakeCoroutine());
    }

    private IEnumerator CameraShakeCoroutine()
    {
        float timeExponential = 1 / ScreenShakeTime;
        while (ScreenShakeTime > 0f)
        {
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
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, -10), Time.deltaTime * 8f) + CameraOffset;
        Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -10, 9.5f), Mathf.Clamp(Camera.main.transform.position.y, -6.88f, 6.25f), Camera.main.transform.position.z);
    }

    public void TakeDamage(int damage)
    {
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
        MaxExp = Level ^ 3 + 50;
        AudioController.PlaySound(AudioController.instance.sounds.playerLvlUpSfx);
        CalculateStats();
    }

    public void Die()
    {
        isDead = true;
        StopAllCoroutines();
        CameraOffset = Vector3.zero;
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
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
