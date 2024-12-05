using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedNecklaceItem : PlayerItem
{
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CursedNecklaceItemEnhancement() };
    }

    public override string getId()
    {
        return "cursednecklace";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>Increases the duration and damage of status effects</color>\n\n";
        description += AddStat("Burning Damage", 12, true, "%");
        description += AddStat("Burning Duration", 4, true, " Beats");
        description += AddStat("Poison Damage", 12, true, "%");
        description += AddStat("Poison Duration", 4, true, " Beats");

        return description;
    }

    public override string getItemName()
    {
        return "Cursed Necklace";
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
        ability.itemValues["burnDuration"] += 4;
        ability.itemValues["burnDamage"] += 0.12f;
    }

    public override void OnUnequip(PlayerAbility ability, int originalSlot)
    {
        base.OnUnequip(ability, originalSlot);
        ability.itemValues["burnDuration"] -= 2;
        ability.itemValues["burnDamage"] -= 0.12f;
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
