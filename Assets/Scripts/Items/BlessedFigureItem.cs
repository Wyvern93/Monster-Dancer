using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessedFigureItem : PlayerItem
{
    private int beat;
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new BlessedFigureItemEnhancement() };
    }

    public override string getId()
    {
        return "blessedfigure";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>On critical strike, have a chance to heal by 1.5% of the damage and allows ability heals to critical strike</color>\n\n";
        description += AddStat("Crit Chance", 15, true, "%");

        return description;
    }

    public override string getItemName()
    {
        return "Blessed Figure";
    }

    public override int getRarity()
    {
        return 2;
    }

    public override void OnCast()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        if (BeatManager.isBeat)
        {
            beat--;
        }
    }
    /*
    public override float OnPreHeal(float heal)
    {
        return heal * 1.25f;
    }*/

    public override void OnEquip(PlayerAbility ability, int slot)
    {
        base.OnEquip(ability, slot);
        ability.itemValues["critChance"] += 15f;
    }

    public override void OnUnequip(PlayerAbility ability, int originalSlot)
    {
        base.OnUnequip(ability, originalSlot);
        ability.itemValues["critChance"] -= 15f;
    }

    public override void OnHit(PlayerAbility source, float damage, Enemy target, bool isCritical)
    {
        if (!isCritical) return;
        List<BlessedFigureItem> figures = new List<BlessedFigureItem> ();
        foreach(PlayerItem item in source.equippedItems)
        {
            if (item is BlessedFigureItem) figures.Add((BlessedFigureItem)item);
        }

        if (figures[0] != this) return;
        
        float heal = figures.Count * 1.5f;
        bool doHeal = Random.Range(0, 100f) < 10f;
        if (!doHeal) return;
        Player.instance.Heal(heal, source);
    }
}
