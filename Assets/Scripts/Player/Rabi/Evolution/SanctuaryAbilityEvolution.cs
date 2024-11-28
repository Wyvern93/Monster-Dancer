using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryAbilityEvolution : PlayerAbility, IPlayerAura
{
    public int minCooldown = 1;

    SanctuaryAura aura;

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
        return Localization.GetLocalizedString("ability.rabi.sanctuary.name");
    }
    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new SanctuaryAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "sanctuary";
    }

    public override void OnCast()
    {
        baseCooldown = 16;
        currentCooldown = GetMaxCooldown();
        currentCooldown = -1;
        SanctuaryAura sanctuaryAura = PoolManager.Get<SanctuaryAura>();
        sanctuaryAura.abilitySource = this;
        aura = sanctuaryAura;
        Player.instance.despawneables.Add(sanctuaryAura);
    }

    public override bool onPlayerPreHurt(float dmg)
    {
        if (aura != null)
        {
            if (!aura.gameObject.activeSelf) return true;
            aura.OnLoseShield();
            aura = null;
            // Negate damage
            currentCooldown = 16;
            return false;
        }
        else
        {
            return true;
        }
        
    }

    public override void OnEquip()
    {

    }
    public override bool isEvolved()
    {
        return true;
    }
    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }

    public override Type getEvolutionAbilityType()
    {
        return null;
    }
}