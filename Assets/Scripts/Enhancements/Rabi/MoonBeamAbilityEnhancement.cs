using JetBrains.Annotations;
using UnityEngine;

public class MoonBeamAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Shoot a moonlight ball of lasers that deal damage to enemies";
    }

    public override string getName()
    {
        return "Moon Beam";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override string getId()
    {
        return "moonbeam";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override PlayerAbility getAbility()
    {
        return new MoonBeamAbility();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}