using JetBrains.Annotations;
using UnityEngine;

public class CarrotJuiceAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Throw a Carrot Juice Bottle that spills and deals damage to enemies in contact";
            case 2: return "Increases damage from <color=\"green\">4 -> 6</color>";
            case 3: return "Reduces the cooldown from <color=\"green\">26 -> 12</color> beats";
            case 4: return "Increases damage from <color=\"green\">6 -> 8</color>";
            case 5: return "Increases juice size to <color=\"green\">150%</color>";
            case 6: return "Increases damage from <color=\"green\">8 -> 12</color>";
            case 7: return "Throw <color=\"green\">1 -> 2</color> juice bottles";
        }
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