using JetBrains.Annotations;
using UnityEngine;

public class EclipseAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
         return "Summon an Eclipse that shoots 4 pulses that deal damage and stun enemies and bullets, heal 6% of your HP for every pulse";
    }

    public override string getName()
    {
        return "Eclipse";
    }

    public override string getDescriptionType()
    {
        return "Special";
    }

    public override string getId()
    {
        return "eclipse";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override PlayerAbility getAbility()
    {
        return new EclipseAbility();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}