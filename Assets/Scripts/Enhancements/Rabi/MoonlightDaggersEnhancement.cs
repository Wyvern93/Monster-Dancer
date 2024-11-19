using JetBrains.Annotations;
using UnityEngine;

public class MoonlightDaggersEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1:
                return "Shoots two waves of moon energy towards the cursor";
            case 2:
                return "Increases weapon damage from <color=\"green\">12 -> 16</color>";
            case 3:
                return "Attacks last <color=\"green\">0.25s -> 0.5s</color>";
            case 4:
                return "Reduce the cooldown of attacks from <color=\"green\">3 -> 2</color> beats";
            case 5:
                return "Increase attacks size by <color=\"green\">30%</color>";
            case 6:
                return "Increases weapon damage from <color=\"green\">16 -> 20</color>";
        }
    }

    public override string getId()
    {
        return "moonlightdaggers";
    }

    public override string getName()
    {
        return "Moonlight Daggers";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 4;
    }

    public override bool isAvailable()
    {
        return Player.instance.abilityValues[$"ability.{getId()}.level"] < 6;
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey($"ability.{getId()}.level")) return 0;
        else return (int)Player.instance.abilityValues[$"ability.{getId()}.level"];
    }

    public override void OnEquip()
    {
        base.OnEquip();

        int lv = getLevel();

        Player.instance.abilityValues["Attack_Number"] = 2;
        Player.instance.abilityValues["Attack_Size"] = lv < 5 ? 1f : 1.3f;
        Player.instance.abilityValues["Attack_Damage"] = lv < 6 ? lv < 2 ? 12 : 16 : 20;
        Player.instance.abilityValues["Attack_Velocity"] = 1;
        Player.instance.abilityValues["Attack_Time"] = lv < 3 ? 0.25f : 0.5f;
        Player.instance.abilityValues["Attack_Cooldown"] = lv < 4 ? 2 : 1;
        Player.instance.abilityValues["Attack_Pierce"] = 9999;

        Player.instance.CalculateStats();
    }

    public override PlayerAbility getAbility()
    {
        return new MoonlightDaggersAbility();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}