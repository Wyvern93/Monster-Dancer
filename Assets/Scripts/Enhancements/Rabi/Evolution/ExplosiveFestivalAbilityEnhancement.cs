using System;
using UnityEngine;

public class ExplosiveFestivalAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Shoots homing carrot fireworks that explode into more explosions";
    }

    public override string getName()
    {
        return "Explosive Festival";
    }

    public override string getId()
    {
        return "explosivefestival";
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
        return new ExplosiveFestialAbilityEvolution();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.EvolvedAbility;
    }

    public override Type getEvolutionItemType()
    {
        return typeof(FireworksKitItem);
    }
}