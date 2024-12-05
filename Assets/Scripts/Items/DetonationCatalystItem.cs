using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class DetonationCatalystItem : PlayerItem
{
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new DetonationCatalystItemEnhancement() };
    }

    public override string getId()
    {
        return "detonationcatalyst";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>All explosions deal bonus percent damage based on their bonus size</color>\n\n";
        description += AddStat("Explosion Size", 10, true, "%");

        return description;
    }

    public override string getItemName()
    {
        return "Detonation Catalyst";
    }

    public override int getRarity()
    {
        return 3;
    }

    public override void OnCast()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEquip(PlayerAbility ability, int slot)
    {
        base.OnEquip(ability, slot);
        ability.itemValues["explosionSize"] += 0.20f;
    }

    public override void OnUnequip(PlayerAbility ability, int originalSlot)
    {
        base.OnUnequip(ability, originalSlot);
        ability.itemValues["explosionSize"] -= 0.20f;
    }

    public override void OnUpdate()
    {

    }

    public override void OnHit(PlayerAbility source, float damage, Enemy target)
    {
        /*
        if (source is IPlayerAura)
        {
        }
        */
    }
}
