using JetBrains.Annotations;
using UnityEngine;

public class IllusionDashAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Controllable <color=\"green\">invulnerable</color> dash that dealing damage around and destroys projectiles";
        }
        if (level == 2)
        {
            return "Increase damage by <color=\"green\">25%</color>";
        }
        if (level == 3)
        {
            return "Decrease the cooldown by <color=\"green\">20%</color>";
        }
        if (level == 4)
        {
            return "Increase speed by <color=\"green\">25%</color>";
        }
        if (level == 5)
        {
            return "Increase damage by <color=\"green\">33%</color>";
        }
        if (level == 6)
        {
            return "Decrease the cooldown by <color=\"green\">25%</color>";
        }
        if (level == 7)
        {
            return "Knock back enemies and last <color=\"green\">66%</color> more time";
        }
        return "";
    }

    public override string getId()
    {
        return "illusiondash";
    }

    public override string getName()
    {
        return "Illusion Dash";
    }

    public override string getDescriptionType()
    {
        return "Active";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override PlayerAbility getAbility()
    {
        return new IllusionDashAbility();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}