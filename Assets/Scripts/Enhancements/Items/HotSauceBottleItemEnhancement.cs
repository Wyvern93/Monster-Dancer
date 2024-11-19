using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class HotSauceBottleItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        float damage = Player.instance.itemValues["burnDamage"] * 100;
        return $"Increases damage from burning effects from <color=\"green\">{damage} -> {damage + 50}%</color>. Additionally, <color=\"blue\">Projectiles</color> burn enemies";
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
        return "Evolution Item";
    }

    public override int getWeight()
    {
        return 1;
    }

    public override bool isAvailable()
    {
        // PASSIVES
        bool available = false;
        if (Player.instance.equippedItems.Count == 6) return available = false;
        if (Player.instance.equippedItems.Find(x => x.getId() == getId()) != null) return available = false;
        if (Player.instance.equippedPassiveAbilities.Any(x => x.GetType() == typeof(PiercingShotAbility))) return true;
        if (Player.instance.equippedPassiveAbilities.Any(x => x.GetType() == typeof(OrbitalMoonAbility))) return true;
        return available;
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
        return EnhancementType.EvolutionItem;
    }

    public override void OnEquip()
    {
        base.OnEquip();
        Player.instance.itemValues["burnDamage"] += 0.5f;
    }
}
