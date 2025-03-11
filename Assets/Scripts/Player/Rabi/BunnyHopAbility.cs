using System.Collections.Generic;
using UnityEngine;

public class BunnyHopAbility : PlayerAbility
{
    public BunnyHopAbility() : base()
    {
        baseCooldown = 4;
    }

    public override float GetMaxCooldown()
    {
        return baseCooldown;
    }

    public override bool CanCast()
    {
        if (((PlayerRabi)Player.instance).isCastingBunnyHop) return false;
        return currentCooldown == 0;
    }

    public override void OnChange()
    {
        
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
        return new List<Enhancement>() { };
    }

    public override string getId()
    {
        return "rabi.bunnyhop";
    }

    public override bool isUltimate()
    {
        return false;
    }
    public override void OnCast()
    {
        currentCooldown = baseCooldown;

        PlayerRabi rabi = (PlayerRabi)Player.instance;
        rabi.DoBunnyHop();
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isBeat && currentCooldown > 0) currentCooldown -= 1f;
    }
}