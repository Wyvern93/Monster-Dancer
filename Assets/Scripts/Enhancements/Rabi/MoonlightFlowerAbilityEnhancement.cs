using JetBrains.Annotations;
using UnityEngine;

public class MoonlightFlowerAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Flowers of Moonlight orbit the screen from afar, dealing damage to enemies";
            case 2: return "Increase damage from <color=\"green\">3 -> 6</color>";
            case 3: return "Lasts for <color=\"green\">6 -> 12</color> beats";
            case 4: return "Increase damage from <color=\"green\">6 -> 12</color>";
            case 5: return "Spawns <color=\"green\">5 -> 8</color> flowers";
            case 6: return "Reduce cooldown from <color=\"green\">20 -> 15</color> beats";
            case 7: return "Flowers have a small knockback on hit";
        }
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