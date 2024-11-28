using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class HotSauceBottleItem : PlayerItem
{
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new HotSauceBottleItemEnhancement() };
    }

    public override string getId()
    {
        return "hotsaucebottle";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>Enemies burn on-hit</color>\n\n";
        description += AddStat("Burn Damage", 25, true, "%");
        description += AddStat("Burn Duration", 2f, true, " Beats");

        return description;
    }

    public override string getItemName()
    {
        return "Hot Sauce Bottle";
    }

    public override int getRarity()
    {
        return 1;
    }

    public override void OnCast()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEquip(PlayerAbility ability, int slot)
    {
        base.OnEquip(ability, slot);
        ability.itemValues["burnDamage"] += 0.25f;
        ability.itemValues["burnDuration"] += 2f;
    }

    public override void OnUnequip(PlayerAbility ability, int originalSlot)
    {
        base.OnUnequip(ability, originalSlot);
        ability.itemValues["burnDamage"] -= 0.25f;
        ability.itemValues["burnDuration"] -= 2f;
    }

    public override void OnUpdate()
    {
    }

    public override void OnHit(PlayerAbility source, float damage, Enemy target)
    {
        target.OnBurn(source, 4f * source.itemValues["burnDamage"], source.itemValues["burnDuration"]);
        //if (source is IPlayerProjectile)
    }
}
