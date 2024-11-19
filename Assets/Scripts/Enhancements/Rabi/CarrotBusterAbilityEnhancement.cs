using UnityEngine;

public class CarrotBusterAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Slash with a giant carrot sword with great power";
    }

    public override string getName()
    {
        return "Carrot Buster";
    }

    public override string getId()
    {
        return "carrotbuster";
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
        return new CarrotBusterAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}