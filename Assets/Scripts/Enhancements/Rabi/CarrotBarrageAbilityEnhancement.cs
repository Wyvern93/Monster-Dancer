using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class CarrotBarrageAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Throw explosive carrots around Rabi that deal damage in an area";
    }

    public override string getName()
    {
        return "Carrot Barrage";
    }

    public override string getId()
    {
        return "carrotbarrage";
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
        return new CarrotBarrageAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}