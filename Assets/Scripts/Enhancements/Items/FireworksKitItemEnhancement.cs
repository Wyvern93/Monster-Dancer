using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FireworksKitItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return $"Increases explosion damage by <color=\"green\">25%</color> and critical strikes have a <color=\"green\">20%</color> chance to cause another explosion";
    }

    public override string getName()
    {
        return "Fireworks Kit";
    }

    public override string getId()
    {
        return "fireworkskit";
    }

    public override string getDescriptionType()
    {
        return "Item";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override bool isAvailable()
    {
        return base.isAvailable();
    }

    public override PlayerAbility getAbility()
    {
        return new CarrotBusterAbility();
    }

    public override PlayerItem getItem()
    {
        return new FireworksKitItem();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Item;
    }

    public override void OnEquip()
    {
        base.OnEquip();
        //Player.instance.itemValues["explosionDamage"] += 0.25f;
    }
}
