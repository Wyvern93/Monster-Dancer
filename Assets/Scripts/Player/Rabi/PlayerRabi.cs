using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRabi : Player
{
    [SerializeField] GameObject explosiveCarrot;
    [SerializeField] GameObject carrotExplosionPrefab;
    [SerializeField] GameObject rabiClonePrefab;
    [SerializeField] GameObject smokeExplosionPrefab;

    [SerializeField] GameObject moonBeamPrefab;

    private bool isCastingBunnyHop;
    protected override void Awake()
    {
        base.Awake();

        animator.speed = 1 / BeatManager.GetBeatDuration();
        animator.Play("Rabi_Idle");
    }

    public override void Start()
    {
        base.Start();
        PoolManager.CreatePool(typeof(RabiAttack), attackPrefab, 4);
        PoolManager.CreatePool(typeof(ExplosiveCarrot), explosiveCarrot, 10);
        PoolManager.CreatePool(typeof(SmokeExplosion), smokeExplosionPrefab, 4);
        PoolManager.CreatePool(typeof(RabiClone), rabiClonePrefab, 4);
        PoolManager.CreatePool(typeof(CarrotExplosion), carrotExplosionPrefab, 10);
        PoolManager.CreatePool(typeof(MoonBeam), moonBeamPrefab, 4);

        GameManager.runData.posibleEnhancements.AddRange(new List<Enhancement>() 
        { 
            new RabiAttackSizeEnhancement(),
            new RabiAttackProjectilesEnhancement(),
            new RabiAttackVelocityEnhancement(),
            new RabiAttackTimeEnhancement(),
            new CarrotBarrageAbilityEnhancement(),
            new BunnyHopAbilityEnhancement(),
            new MoonBeamAbilityEnhancement()
        });
        //ultimateAbility = new MoonBeamAbility();
        //ultimateAbility.OnEquip();
        //instance.abilityValues.Add("ability.moonbeam.level", 1);
        //CurrentSP = MaxSP;

        abilityValues.Add("Attack_Number", 2); // 4 Upgrades
        abilityValues.Add("Attack_Size", 1f); // 20 Upgrades
        abilityValues.Add("Attack_Velocity", 1); // 20 Upgrades
        abilityValues.Add("Attack_Time", 1f); // 10 Upgrades

        abilityValues.Add("Max_Attack_Number", 10);
        abilityValues.Add("Max_Attack_Size", 3);
        abilityValues.Add("Max_Attack_Velocity", 3);
        abilityValues.Add("Max_Attack_Time", 4);

        UIManager.Instance.PlayerUI.SetWeaponIcon(IconList.instance.moonlightDaggers, 1);
    }

    public override void Despawn()
    {
        PoolManager.RemovePool(typeof(RabiAttack));
        PoolManager.RemovePool(typeof(ExplosiveCarrot));
        PoolManager.RemovePool(typeof(CarrotExplosion));
        Destroy(gameObject);
    }

    protected override IEnumerator DeathCoroutine()
    {
        BeatManager.Stop();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Sprite.sortingLayerName = "UI";
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
            direction = oldDir;
            targetPos = (Vector2)originPos + oldDir;
        }
        if (Map.isWallAt(targetPos)) targetPos = originPos;

        position = targetPos;

        animator.Play("Rabi_Move");
        animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 1.5f);

        while (time <= BeatManager.GetBeatDuration() / 3f)
        {
            if (isCastingBunnyHop)
            {
                position = originPos;
                yield break;
            }
            
            if (Map.isWallAt(targetPos)) targetPos = originPos;

            transform.position = Vector3.Lerp(originPos, (Vector3)targetPos, time * 8f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("Rabi_Idle");
        yield return new WaitForEndOfFrame();
        Sprite.transform.localPosition = Vector3.zero;
        transform.position = targetPos;
        position = targetPos;

        isPerformingAction = false;
        isMoving = false;
        yield break;
    }

    public override void OnAttack()
    {
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
        StartCoroutine(BunnyHopCoroutine(position + dir));
        
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

        position = targetPos;

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
        animator.Play("Rabi_Idle");
        Sprite.transform.localPosition = Vector3.zero;
        transform.position = targetPos;
        position = targetPos;

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
}
