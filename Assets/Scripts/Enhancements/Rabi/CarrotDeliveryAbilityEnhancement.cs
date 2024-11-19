using UnityEngine;

public class CarrotDeliveryAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Call a Delivery Truck that runs overs enemies and shoots carrot bullets";
    }

    public override string getName()
    {
        return "Carrot Delivery";
    }

    public override string getDescriptionType()
    {
        return "Special";
    }

    public override string getId()
    {
        return "carrotdelivery";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override PlayerAbility getAbility()
    {
        return new CarrotDeliveryAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}