using JetBrains.Annotations;
using UnityEngine;

public class MoonBeamAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Shoot a moonlight ball of lasers that deal damage to enemies";
            case 2: return "Increase damage from <color=\"green\">10 -> 14</color>";
            case 3: return "Increase duration from <color=\"green\">4 -> 6</color> beats";
            case 4: return "Increase damage from <color=\"green\">14 -> 20</color>";
            case 5: return "<color=\"green\">1 -> 2</color> moonlight balls spawn";
            case 6: return "Increase size by <color=\"green\">25%</color>";
            case 7: return "Moonballs casts moon echos when hitting enemies";
        }
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