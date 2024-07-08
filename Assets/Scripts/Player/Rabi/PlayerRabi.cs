using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRabi : Player
{

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

        abilityValues.Add("Attack_Number", 2); // 4 Upgrades
        abilityValues.Add("Attack_Size", 1f); // 20 Upgrades
        abilityValues.Add("Attack_Velocity", 2); // 20 Upgrades

        abilityValues.Add("Max_Attack_Number", 10);
        abilityValues.Add("Max_Attack_Size", 3);
        abilityValues.Add("Max_Attack_Velocity", 3);
    }

    public override void Despawn()
    {
        PoolManager.RemovePool(typeof(RabiAttack));
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

        animator.Play("Rabi_Move");
        animator.SetFloat("animatorSpeed", 1f / (BeatManager.GetBeatDuration() / 2f));

        while (time <= BeatManager.GetBeatDuration() / 4f)
        {
            if (Map.isWallAt(targetPos)) targetPos = originalPos;

            transform.position = Vector3.Lerp(originalPos, (Vector3)targetPos, time * 8f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.Play("Rabi_Idle");
        Sprite.transform.localPosition = Vector3.zero;
        transform.position = targetPos;
        position = targetPos;

        isPerformingAction = false;
        yield break;
    }

    public override void OnAttack()
    {
        if (direction.x < 0) facingRight = false;
        else facingRight = true;
        StartCoroutine(AttackCoroutine());
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
}
