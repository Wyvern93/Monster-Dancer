using System.Drawing;
using UnityEngine;

public class CarrotcopterAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "A carrot shaped drone that shoots carrot bullets with a main focus on stronger enemies";
            case 2: return "Increases bullet damage from <color=\"green\">3 -> 6</color>";
            case 3: return "Increases movement speed by <color=\"green\">25%</color>";
            case 4: return "Increases bullet damage from <color=\"green\">6 -> 12</color>";
            case 5: return "Increases movement speed by <color=\"green\">25%</color>";
            case 6: return "Increases ammo from <color=\"green\">4 -> 8</color>";
            case 7: return "Carrot bullets now <color=\"green\">pierce</color> enemies";
        }
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