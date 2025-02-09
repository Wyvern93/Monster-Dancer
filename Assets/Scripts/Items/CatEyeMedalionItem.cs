using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class CatEyeMedalionItem : PlayerItem
{
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CatEyeMedalionItemEnhancement() };
    }

    public override string getId()
    {
        return "cateyemedalion";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>Increases ability reach range and reduces cooldown</color>\n\n";
        description += AddStat("Reach", 25f, true, "%");
        description += AddStat("Cooldown", -20f, false, "%");

        return description;
    }

    public override string getItemName()
    {
        return "Cat Eye Medalion";
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
        ability.itemValues["reachMultiplier"] += 0.25f;
        ability.itemValues["cooldownMultiplier"] -= 0.20f;
    }

    public override void OnUnequip(PlayerAbility ability, int originalSlot)
    {
        base.OnUnequip(ability, originalSlot);
        ability.itemValues["reachMultiplier"] -= 0.25f;
        ability.itemValues["cooldownMultiplier"] += 0.20f;
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
