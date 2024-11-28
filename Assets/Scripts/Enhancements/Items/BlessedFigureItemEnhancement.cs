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
        return new BlessedFigureItem();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Item;
    }
}
