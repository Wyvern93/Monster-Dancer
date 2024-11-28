using System.Drawing;
using UnityEngine;

public class CarrotcopterAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "A carrot shaped drone that shoots carrot bullets with a main focus on stronger enemies";
    }

    public override string getName()
    {
        return "Carrotcopter";
    }

    public override string getId()
    {
        return "carrotcopter";
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
        return new CarrotcopterAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}