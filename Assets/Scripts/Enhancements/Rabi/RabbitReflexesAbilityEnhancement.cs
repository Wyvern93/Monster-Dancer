using JetBrains.Annotations;
using UnityEngine;

public class RabbitReflexesAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Grazing bullets makes them shine, when the bullet despawns, grants a charge of <color=\"yellow\">2%</color> attack damage up to 20 charges and lose <color=\"yellow\">50%</color> charges on hit";
        }
        if (level == 2)
        {
            return "Increases graze range by <color=\"yellow\">50%</color>";
        }
        if (level == 3)
        {
            return "Gain <color=\"yellow\">2->5%</color> attack damage per charge";
        }
        if (level == 4)
        {
            return "Grazing bullets also reduce bullets' lifetime by <color=\"yellow\">50%</color>";
        }
        return "";
    }

    public override string getId()
    {
        return "rabbitreflexes";
    }

    public override string getName()
    {
        return "Rabbit Reflexes";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override PlayerAbility getAbility()
    {
        return new RabbitReflexesAbility();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}