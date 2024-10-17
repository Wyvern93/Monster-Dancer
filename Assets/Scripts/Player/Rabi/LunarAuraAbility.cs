using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LunarAuraAbility : PlayerAbility, IPlayerAura
{
    public int minCooldown = 1;
    int level;

    LunarAura aura;

    public override bool CanCast()
    {
        return currentCooldown == 0;
    }

    public override string getAbilityDescription()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDespawn()
    {
        Player.instance.despawneables.FirstOrDefault(x => x.GetType() == typeof(LunarAura)).ForceDespawn();
    }

    public override string getAbilityName()
    {
        return Localization.GetLocalizedString("ability.rabi.lunaraura.name");
    }
    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new LunarAuraAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "lunaraura";
    }

    public override void OnCast()
    {
        level = (int)Player.instance.abilityValues["ability.lunaraura.level"];
        maxCooldown = level < 3 ? 16 : 12;
        currentCooldown = maxCooldown;
        if (level >= 7) currentCooldown = -1;
        LunarAura lunarAura = PoolManager.Get<LunarAura>();
        aura = lunarAura;
        Player.instance.despawneables.Add(lunarAura);
    }

    public override bool onPlayerPreHurt(float dmg)
    {
        if (aura != null)
        {
            if (!aura.gameObject.activeSelf) return true;
            aura.OnLoseShield();
            aura = null;
            // Negate damage
            if (level >= 7) currentCooldown = 12;
            else currentCooldown = maxCooldown;
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

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }

    public override System.Type getEvolutionItemType()
    {
        return typeof(BlessedFigureItem);
    }

    public override Enhancement getEvolutionEnhancement()
    {
        return new SanctuaryAbilityEnhancement();
    }
}