using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class CarrotBarrageAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Throw explosive carrots around Rabi that deal damage in an area";
            case 2: return "Increases damage from <color=\"green\">25 -> 40</color>";
            case 3: return "Reduces the cooldown from <color=\"green\">10 -> 8</color> beats";
            case 4: return "Increases damage from <color=\"green\">40 -> 65</color>";
            case 5: return "Increase the number of carrots from <color=\"green\">3 -> 5</color>";
            case 6: return "Reduces the cooldown from <color=\"green\">8 -> 6</color> beats";
            case 7: return "Carrots explode into smaller explosive carrots that deal 50% damage";
        }
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