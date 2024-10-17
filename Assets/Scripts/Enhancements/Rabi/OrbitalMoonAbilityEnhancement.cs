using JetBrains.Annotations;
using UnityEngine;

public class OrbitalMoonAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "<color=\"green\">2</color> moons orbit Rabi that deal damage";
            case 2: return "Increase damage from <color=\"green\">15 -> 30</color>";
            case 3: return "Spins <color=\"green\">25%</color> faster and orbits for<color=\"green\">6 -> 12</color> beats";
            case 4: return "Increase damage from <color=\"green\">30 -> 45</color>";
            case 5: return "<color=\"green\">2 -> 3</color> moons spawn";
            case 6: return "Increases the moons orbit range by <color=\"green\">25%</color>";
            case 7: return "Moons never disappear and have a small knockback on hit";
        }
    }

    public override string getName()
    {
        return "Orbital Moon";
    }

    public override string getId()
    {
        return "orbitalmoon";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 3;
    }

    public override PlayerAbility getAbility()
    {
        return new OrbitalMoonAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}