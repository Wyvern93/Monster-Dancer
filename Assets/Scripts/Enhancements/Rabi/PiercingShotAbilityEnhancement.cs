using JetBrains.Annotations;
using UnityEngine;

public class PiercingShotAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Charge a powerful spinning carrot that pierces through enemies at high speed in the direction you are aiming";
            case 2: return "Increases damage from <color=\"green\">20 -> 26</color>";
            case 3: return "Reduces the cooldown from <color=\"green\">10 -> 7</color> beats";
            case 4: return "Increases damage from <color=\"green\">26 -> 34</color>";
            case 5: return "Increase size by <color=\"green\">50%</color>";
            case 6: return "Enemies hit are pushed back slightly";
            case 7: return "Every time a Piercing Shot hits an enemy, splits into smaller drills that deal <color=\"green\">75%</color> of the base damage";
        }
    }

    public override string getName()
    {
        return "Piercing Shot";
    }

    public override string getId()
    {
        return "piercingshot";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 3; // How likely to appear
    }

    public override PlayerAbility getAbility()
    {
        return new PiercingShotAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}