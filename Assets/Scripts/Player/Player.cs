using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;
    public bool isPerformingAction;

    private Vector2 direction, oldDir;
    public bool facingRight { get; private set; }

    [SerializeField] private SpriteRenderer Sprite;
    public float SpriteSize = 1f;
    private float SpriteX = 1f;

    public static Vector2 position {  get; private set; }

    public bool onbeat;
    public bool waitForNextBeat;

    private float ScreenShakeStrength;
    private float ScreenShakeTime;
    private Vector2 ScreenShakeDir;
    private Vector3 CameraOffset;
    private PlayerAction action;

    public int MaxHP, CurrentHP;
    public int MaxSP, CurrentSP;
    public int MaxExp, CurrentExp, Level;

    [SerializeField] AudioClip playerHurtSfx;
    [SerializeField] AudioClip playerLvlUpSfx;

    private Material spriteRendererMat;
    private Color emissionColor = new Color(1,0,0,0);

    [SerializeField] PlayerAttack attackEntity;
    bool canAttack = true;

    public bool CanMove()
    {
        if (isPerformingAction) return false;
        else return BeatManager.GetBeatSuccess() != BeatTrigger.FAIL;
    }

    private void Awake()
    {
        instance = this;
        spriteRendererMat = Sprite.material;
        Level = 1;
        MaxExp = Level ^ 3 + 50;
        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.UpdateExp();
        UIManager.Instance.PlayerUI.UpdateSpecial();
    }

    // Start is called before the first frame update
    void Start()
    {
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        HandleSprite();
        HandleCamera();
        BeatManager.SetRingPosition(position);

        if (BeatManager.isGameBeat)
        {
            CurrentSP = Mathf.Clamp(CurrentSP + 1, 0, MaxSP);
            UIManager.Instance.PlayerUI.UpdateSpecial();
        }
        
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    void HandleInput()
    {
        //direction = new Vector2(Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0, Keyboard.current.sKey.isPressed ? -1 : Keyboard.current.wKey.isPressed ? 1 : 0);
        direction.x = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? - 1 : Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
        direction.y = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 0;
        if (direction !=  Vector2.zero) {
            oldDir = direction;
        }
        if (direction.x == -1) facingRight = false;
        if (direction.x == 1) facingRight = true;

        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            action = PlayerAction.Move;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            BeatTrigger result = BeatManager.GetBeatSuccess();
            if (result != BeatTrigger.FAIL && !waitForNextBeat && canAttack)
            {
                attackEntity.Attack(direction);
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

        if (!canAttack && !BeatManager.closestIsNextBeat() && BeatManager.GetBeatSuccess() == BeatTrigger.FAIL)
        {
            canAttack = true;
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
            case PlayerAction.Attack:
                if (direction == Vector2.zero)
                {
                    attackEntity.Attack(oldDir);
                }
                else
                {
                    attackEntity.Attack(direction);
                }
                break;
        }
    }

    public void Move(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
    }

    IEnumerator MoveCoroutine(Vector2 targetPos)
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

    private void HandleSprite()
    {
        SpriteSize = Mathf.MoveTowards(SpriteSize, 1f, Time.deltaTime * 4f);
        SpriteX = Mathf.MoveTowards(SpriteX, facingRight ? 1 : -1, Time.deltaTime * 24f);
        Sprite.transform.localScale = new Vector3(SpriteX, 1, 1) * SpriteSize;
        
        emissionColor = Color.Lerp(emissionColor, new Color(1, 0, 0, 0), Time.deltaTime * 16f);
        spriteRendererMat.SetColor("_EmissionColor", emissionColor);
    }

    private void HandleCamera()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, -10), Time.deltaTime * 8f) + CameraOffset;
        Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -10, 9.5f), Mathf.Clamp(Camera.main.transform.position.y, -6.88f, 6.25f), Camera.main.transform.position.z);
    }

    public void TakeDamage(int damage)
    {
        CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, 9999);
        TriggerCameraShake(0.3f, 0.2f);
        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.DoHurtEffect();
        AudioController.PlaySound(playerHurtSfx);

        emissionColor = new Color(1, 0, 0, 1);

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public static void AddExp(int n)
    {
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
        instance.CurrentSP = Mathf.Clamp(instance.CurrentSP + n, 0, 250);
        UIManager.Instance.PlayerUI.UpdateSpecial();
    }

    public void OnLevelUp()
    {
        MaxExp = Level ^ 3 + 50;
        AudioController.PlaySound(playerLvlUpSfx);
    }

    public void Die()
    {

    }
}
