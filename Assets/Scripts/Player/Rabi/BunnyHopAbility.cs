using System.Collections.Generic;
using UnityEngine;

public class BunnyHopAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;
    public BunnyHopAbility() : base(4)
    {
    }

    public override bool CanCast()
    {
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
        throw new System.NotImplementedException();
    }

    public override string getID()
    {
        return "rabi.bunnyhop";
    }

    public override void OnCast()
    {
        int targetCD = 0;
        level = (int)Player.instance.abilityValues["ability.bunnyhop.level"];

        if (level == 1)
        {
            targetCD = 16;
        }
        else if (level == 2)
        {
            targetCD = 12;
        }
        else if (level == 3)
        {
            targetCD = 8;
        }

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