using JetBrains.Annotations;
using UnityEngine;

public class LunarAuraAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "An aura that deals damage to enemies inside and protects you from a hit";
            case 2: return "Increase damage from <color=\"green\">4 -> 6</color>";
            case 3: return "Reduces cooldown from <color=\"green\">16 -> 12</color> beats";
            case 4: return "Increase damage from <color=\"green\">6 -> 8</color>";
            case 5: return "The aura now pushes enemies";
            case 6: return "Increases the aura size by <color=\"green\">25%</color>";
            case 7: return "The aura only disappears after taking damage";
        }
    }

    public override string getName()
    {
        return "Lunar Aura";
    }

    public override string getId()
    {
        return "lunaraura";
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
        return new LunarAuraAbility();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}