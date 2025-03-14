using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedNecklaceItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return $"Increases duration of damaging status effects by <color=\"green\">2</color> beats";
    }

    public override string getName()
    {
        return "Cursed Necklace";
    }

    public override string getId()
    {
        return "cursednecklace";
    }

    public override string getDescriptionType()
    {
        return "Item";
    }

    public override int getWeight()
    {
        return 3;
    }

    public override PlayerAbility getAbility()
    {
        return null;
    }

    public override PlayerItem getItem()
    {
        return new CursedNecklaceItem();
    }

    public override bool isAvailable()
    {
        return base.isAvailable();
    }

    public override void OnEquip()
    {
        base.OnEquip();
        //Player.instance.itemValues["burnDuration"] += 2;
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Item;
    }
}
