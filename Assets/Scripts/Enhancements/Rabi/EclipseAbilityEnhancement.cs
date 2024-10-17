using JetBrains.Annotations;
using UnityEngine;

public class EclipseAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Summon an Eclipse that shoots 4 pulses that deal damage and stun enemies and bullets, heal 6% of your HP for every pulse";
            case 2: return "Lasts <color=\"green\">1</color> additional pulse";
            case 3: return "Pulses also deal <color=\"green\">5%</color> of enemies max health";
            case 4: return "Increase healing of each pulse from <color=\"green\">6 -> 8%</color> Max Health";
            case 5: return "Lasts <color=\"green\">1</color> additional pulse";
            case 6: return "Increase healing of each pulse from <color=\"green\">8 -> 10%</color> Max Health";
            case 7: return "Pulses also destroy bullets";
        }
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