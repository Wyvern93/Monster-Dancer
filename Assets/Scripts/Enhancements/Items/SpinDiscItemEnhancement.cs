using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpindiscItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        if (!Player.instance.itemValues.ContainsKey("orbitalSpeed")) Player.instance.itemValues.Add("orbitalSpeed", 1.0f);
        if (!Player.instance.itemValues.ContainsKey("orbitalDamage")) Player.instance.itemValues.Add("orbitalDamage", 1.0f);
        float speed = Player.instance.itemValues["orbitalSpeed"] * 100;
        float damage = Player.instance.itemValues["orbitalDamage"] * 100;
        return $"Increases orbital and spin speed from abilities from <color=\"green\">{speed} -> {speed + 10}%</color> and damage from orbital and speed abilities from <color=\"green\">{damage} -> {damage + 10}%</color>";
    }

    public override string getName()
    {
        return "Spin Disc";
    }

    public override string getId()
    {
        return "spindisc";
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
        return new SpinDiscItem();
    }

    public override bool isAvailable()
    {
        return base.isAvailable();
    }

    public override void OnEquip()
    {
        base.OnEquip();
        if (!Player.instance.itemValues.ContainsKey("orbitalSpeed")) Player.instance.itemValues.Add("orbitalSpeed", 1.0f);
        Player.instance.itemValues["orbitalSpeed"] += 0.10f;

        if (!Player.instance.itemValues.ContainsKey("orbitalDamage")) Player.instance.itemValues.Add("orbitalDamage", 1.0f);
        Player.instance.itemValues["orbitalDamage"] += 0.10f;
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.StackableItem;
    }
}
