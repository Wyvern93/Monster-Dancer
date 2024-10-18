using JetBrains.Annotations;
using UnityEngine;

public class LunarRainAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1) return "Lunar beams fall from the sky onto closeby enemies, dealing damage";
        if (level == 2) return "Increases damage from <color=\"green\">15 -> 30</color>";
        if (level == 3) return "Lunar Beams fall every <color=\"green\">4 -> 2</color> beats";
        if (level == 4) return "Increases damage from <color=\"green\">30 -> 50</color>";
        if (level == 5) return "Lunar Beams fall every <color=\"green\">2 -> 1</color> beat";
        if (level == 6) return "<color=\"green\">1 -> 2</color> Lunar Beams fall every beat";
        if (level == 7) return "Killing an enemy with a Lunar Beam cast another beam";
        return "";
    }
    public override string getName()
    {
        return "Lunar Rain";
    }

    public override string getId()
    {
        return "lunarrain";
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
        return new LunarRainAbility();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}