using JetBrains.Annotations;
using UnityEngine;

public class LunarRainAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Lunar beams fall from the sky onto closeby enemies, dealing damage";
    }
    public override string getName()
    {
        return "Lunar Rain";
    }

    public override string getId()
    {
        return "lunarrain";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override PlayerAbility getAbility()
    {
        return new LunarRainAbility();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}