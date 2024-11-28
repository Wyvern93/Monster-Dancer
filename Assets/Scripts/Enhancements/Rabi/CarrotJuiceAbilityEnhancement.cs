using JetBrains.Annotations;
using UnityEngine;

public class CarrotJuiceAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Throw a Carrot Juice Bottle that spills and deals damage to enemies in contact";
    }

    public override string getName()
    {
        return "Carrot Juice";
    }

    public override string getId()
    {
        return "carrotjuice";
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
        return new CarrotJuiceAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}