using System.Collections.Generic;
using UnityEngine;

public class IllusionDashAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;
    public IllusionDashAbility() : base(20)
    {
    }

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

    public override Sprite GetIcon()
    {
        return IconList.instance.illusionDash;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new IllusionDashAbilityEnhancement() };
    }

    public override string getID()
    {
        return "rabi.illusiondash";
    }

    public override void OnCast()
    {
        level = (int)Player.instance.abilityValues["ability.illusiondash.level"];

        maxCooldown = level < 4 ? 20 : 10;
        int tiles = level < 2 ? 4 : 5;

        currentCooldown = maxCooldown;

        PlayerRabi rabi = (PlayerRabi)Player.instance;
        rabi.DoIllusionDash(tiles);
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}