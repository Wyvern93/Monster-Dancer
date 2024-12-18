using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatEyeMedalionItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return $"Increases ability reach range and reduces cooldown</color>";
    }

    public override string getName()
    {
        return "Cat Eye Medalion";
    }

    public override string getId()
    {
        return "cateyemedalion";
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
        return new CatEyeMedalionItem();
    }

    public override bool isAvailable()
    {
        return base.isAvailable();
    }

    public override void OnEquip()
    {
        base.OnEquip();
        /*
        if (!Player.instance.itemValues.ContainsKey("orbitalSpeed")) Player.instance.itemValues.Add("orbitalSpeed", 1.0f);
        Player.instance.itemValues["orbitalSpeed"] += 0.10f;

        if (!Player.instance.itemValues.ContainsKey("orbitalDamage")) Player.instance.itemValues.Add("orbitalDamage", 1.0f);
        Player.instance.itemValues["orbitalDamage"] += 0.10f;
        */
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Item;
    }
}
