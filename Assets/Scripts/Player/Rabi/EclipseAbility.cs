using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EclipseAbility : PlayerAbility
{
    public float healing;
    public EclipseAbility() : base()
    {
        baseCooldown = 250;
        baseDamage = 30;
        healing = 0.08f;

        currentCooldown = 0;
    }
    public override bool CanCast()
    {
        return currentCooldown <= 0;
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Summon an eclipse that pulses 4 times.\n\nEach pulse deals damage, stuns enemies, destroys bullets and heals the player</color>\n\n";
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Pulse Damage", baseDamage, GetDamage(), true," + 5% Enemy Max HP");
        description += AddStat("Pulse Healing", healing * 100, healing * 100, true, "% Max HP");

        return description;
    }

    public override Color GetTooltipColor()
    {
        return new Color(0, 0.5f, 1f);
    }

    public override string getAbilityName()
    {
        return "Eclipse";
    }

    public override void OnChange()
    {
        
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new EclipseAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return true;
    }
    public override string getId()
    {
        return "eclipse";
    }

    public override void OnCast()
    {
        currentCooldown = GetMaxCooldown();
        UIManager.Instance.PlayerUI.UpdateSpecial(currentCooldown, GetMaxCooldown());
        Player.instance.StartCoroutine(UltimateCast());
    }

    public IEnumerator UltimateCast()
    {
        PlayerRabi rabi = (PlayerRabi)Player.instance;
        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        rabi.animator.Play("Rabi_Ultimate");

        AudioController.PlaySound(AudioController.instance.sounds.playerSpecialUseSfx);


        RabiEclipse eclipse = PoolManager.Get<RabiEclipse>();
        eclipse.dmg = GetDamage();
        eclipse.healing = healing;
        Player.instance.despawneables.Add(eclipse);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        rabi.animator.Play("Rabi_Idle");
        yield break;
    }

    public override void OnEquip()
    {
        UIManager.Instance.PlayerUI.UpdateSpecial(currentCooldown, GetMaxCooldown());
    }

    public override void OnUpdate()
    {
        //if (BeatManager.isQuarterBeat && currentCooldown > 0) currentCooldown -= 0.25f;
        UIManager.Instance.PlayerUI.UpdateSpecial(currentCooldown, GetMaxCooldown());
    }
}