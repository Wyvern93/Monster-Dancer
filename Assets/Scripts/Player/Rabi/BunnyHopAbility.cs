using System.Collections.Generic;
using UnityEngine;

public class BunnyHopAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;
    public BunnyHopAbility() : base(6) // 20
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
        return Localization.GetLocalizedString("ability.rabi.bunnyhop.name");
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.bunnyhop;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new BunnyHopAbilityEnhancement() };
    }

    public override string getID()
    {
        return "rabi.bunnyhop";
    }

    public override void OnCast()
    {
        level = (int)Player.instance.abilityValues["ability.bunnyhop.level"];
        if (level == 1)
        {
            maxCooldown = 20;
        }
        else if (level == 2)
        {
            maxCooldown = 16;
        }
        else if (level >= 3)
        {
            maxCooldown = 12;
        }
        maxCooldown = 6;
        currentCooldown = maxCooldown;

        PlayerRabi rabi = (PlayerRabi)Player.instance;
        rabi.DoBunnyHop();
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}