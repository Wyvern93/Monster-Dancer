using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class HotSauceBottleItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return $"Increases damage from burning effects by <color=\"green\">50%</color>. Additionally, <color=\"blue\">Projectiles</color> burn enemies";
    }

    public override string getName()
    {
        return "Hot Sauce Bottle";
    }

    public override string getId()
    {
        return "hotsaucebottle";
    }

    public override string getDescriptionType()
    {
        return "Item";
    }

    public override int getWeight()
    {
        return 1;
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
        return new HotSauceBottleItem();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Item;
    }

    public override void OnEquip()
    {
        base.OnEquip();
        //Player.instance.itemValues["burnDamage"] += 0.5f;
    }
}
