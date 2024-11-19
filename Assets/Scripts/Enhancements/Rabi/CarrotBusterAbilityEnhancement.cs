using UnityEngine;

public class CarrotBusterAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Slash with a giant carrot sword with great power";
            case 2: return "Increases damage from <color=\"green\">30 -> 42</color>";
            case 3: return "Reduces the cooldown from <color=\"green\">18->12</color> beats";
            case 4: return "Increases damage from <color=\"green\">42 -> 59</color>";
            case 5: return "Increase healing from <color=\"green\">5% -> 10%</color> Max HP";
            case 6: return "Increases damage from <color=\"green\">59 -> 78</color>";
            case 7: return "<color=\"green\">1->2</color> boxes fall every time";
        }
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