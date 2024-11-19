using JetBrains.Annotations;
using UnityEngine;

public class BunnyHopAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Rabi can jump over tiles and obstacles, leaving behind a perfectly identical clone that lasts for <color=\"yellow\">20</color> beats.";
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