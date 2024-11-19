using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FireworksKitItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        float damage = Player.instance.itemValues["explosionDamage"] * 100;
        return $"Increases explosion damage from <color=\"green\">{damage} -> {damage + 25}%</color> and critical strikes have a <color=\"green\">20%</color> chance to cause another explosion";
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
        if (Player.instance.equippedPassiveAbilities.Any(x => x.GetType() == typeof(CarrotBarrageAbility))) return true;
        if (Player.instance.equippedPassiveAbilities.Any(x => x.GetType() == typeof(CarrotBusterAbility))) return true;
        return available;
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
        Player.instance.itemValues["explosionDamage"] += 0.25f;
    }
}
