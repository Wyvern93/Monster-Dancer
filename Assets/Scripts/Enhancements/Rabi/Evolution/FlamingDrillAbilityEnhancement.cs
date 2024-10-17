using JetBrains.Annotations;
using System;
using UnityEngine;

public class FlamingDrillAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "A flaming carrot that explodes into a blast of flames upon contact and burns enemies";
    }

    public override string getName()
    {
        return "Flaming Drill";
    }

    public override string getId()
    {
        return "flamingdrill";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 4;
    }
    public override PlayerAbility getAbility()
    {
        return new FlamingDrillAbilityEvolution();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.EvolvedAbility;
    }

    public override Type getEvolutionItemType()
    {
        return typeof(HotSauceBottleItem);
    }
}