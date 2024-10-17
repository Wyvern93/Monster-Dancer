using JetBrains.Annotations;
using UnityEngine;

public class BunnyHopAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Rabi can jump over tiles and obstacles, leaving behind a perfectly identical clone that lasts for <color=\"yellow\">20</color> beats.";
        }
        if (level == 2)
        {
            return "Reduces the cooldown from <color=\"yellow\">20 -> 16</color> beats.";
        }
        if (level == 3)
        {
            return "Reduces the cooldown from <color=\"yellow\">16 -> 12</color> beats.";
        }
        if (level == 4)
        {
            return "Rabi's perfectly identical clone lasts <color=\"yellow\">12 -> 20</color> beats.";
        }
        return "";
    }

    public override string getId()
    {
        return "bunnyhop";
    }

    public override string getName()
    {
        return "Bunny Hop";
    }

    public override string getDescriptionType()
    {
        return "Active";
    }

    public override int getWeight()
    {
        return 3;
    }
    public override PlayerAbility getAbility()
    {
        return new BunnyHopAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}