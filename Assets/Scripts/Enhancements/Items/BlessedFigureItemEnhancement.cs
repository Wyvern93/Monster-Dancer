using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BlessedFigureItemEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Increases healing from abilities by <color=\"green\">25%</color>. Additionally, heals can critically strike";
    }

    public override string getName()
    {
        return "Blessed Figure";
    }

    public override string getId()
    {
        return "blessedfigure";
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
        if (Player.instance.equippedPassiveAbilities.Any(x => x.GetType() == typeof(LunarAuraAbility))) return true;
        if (Player.instance.equippedPassiveAbilities.Any(x => x.GetType() == typeof(CarrotJuiceAbility))) return true;
        return available;
    }

    public override PlayerAbility getAbility()
    {
        return new BoxOfCarrotsAbility();
    }

    public override PlayerItem getItem()
    {
        return new BlessedFigureItem();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.EvolutionItem;
    }
}
