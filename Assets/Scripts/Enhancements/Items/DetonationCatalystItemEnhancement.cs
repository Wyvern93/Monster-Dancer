using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetonationCatalystItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        float size = Player.instance.itemValues["explosionSize"] * 100;
        return $"Increases size of explosions from <color=\"green\">{size} -> {size + 10}%</color>, additionally, all explosions deal damage based on their size";
    }

    public override string getName()
    {
        return "Detonation Catalyst";
    }

    public override string getId()
    {
        return "detonationcatalyst";
    }

    public override string getDescriptionType()
    {
        return "Item";
    }

    public override int getWeight()
    {
        return 4;
    }

    public override PlayerAbility getAbility()
    {
        return null;
    }

    public override PlayerItem getItem()
    {
        return new DetonationCatalystItem();
    }

    public override bool isAvailable()
    {
        return base.isAvailable();
    }

    public override void OnEquip()
    {
        base.OnEquip();
        Player.instance.itemValues["explosionSize"] += 0.1f;
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Item;
    }
}
