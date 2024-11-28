using JetBrains.Annotations;
using UnityEngine;

public class MoonlightFlowerAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Flowers of Moonlight orbit the screen from afar, dealing damage to enemies";
    }

    public override string getName()
    {
        return "Moonlight Flower";
    }

    public override string getId()
    {
        return "moonlightflower";
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
        return new MoonlightFlowerAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}