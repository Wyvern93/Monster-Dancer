using System.Collections.Generic;
using UnityEngine;

public class IllusionDashAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;

    public override bool CanCast()
    {
        if (((PlayerRabi)Player.instance).isCastingBunnyHop) return false;
        return currentCooldown == 0;
    }

    public override string getAbilityDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string getAbilityName()
    {
        return Localization.GetLocalizedString("ability.rabi.illusiondash.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new IllusionDashAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "illusiondash";
    }

    public override void OnCast()
    {
        level = (int)Player.instance.abilityValues["ability.illusiondash.level"];

        maxCooldown = level < 6 ? level < 3 ? 24 : 20 : 16;

        currentCooldown = maxCooldown;

        PlayerRabi rabi = (PlayerRabi)Player.instance;
        rabi.DoIllusionDash(level);
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}