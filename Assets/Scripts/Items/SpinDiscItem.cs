using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SpinDiscItem : PlayerItem
{
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new SpindiscItemEnhancement() };
    }

    public override string getId()
    {
        return "spindisc";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>Speeds up cooldowns and attack speed at the cost of damage</color>\n\n";
        description += AddStat("Cooldown", -0.5f, false, " Beats");
        description += AddStat("Attack Speed", -0.5f, false, " Beats");
        description += AddStat("Damage", -12, true, "%");

        return description;
    }

    public override string getItemName()
    {
        return "Spin Disc";
    }

    public override int getRarity()
    {
        return 4;
    }

    public override void OnCast()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEquip(PlayerAbility ability, int slot)
    {
        base.OnEquip(ability, slot);
        ability.itemValues["damageMultiplier"] -= 0.12f;
        ability.itemValues["bonusCooldown"] -= 0.5f;
        ability.itemValues["bonusAtkSpeed"] -= 0.5f;
    }

    public override void OnUnequip(PlayerAbility ability, int originalSlot)
    {
        base.OnUnequip(ability, originalSlot);
        ability.itemValues["damageMultiplier"] += 0.12f;
        ability.itemValues["bonusCooldown"] += 0.5f;
        ability.itemValues["bonusAtkSpeed"] += 0.5f;
    }

    public override void OnUpdate()
    {

    }

    public override void OnHit(PlayerAbility source, float damage, Enemy target, bool isCritical)
    {
        /*
        if (source is IPlayerAura)
        {
        }
        */
    }
}
