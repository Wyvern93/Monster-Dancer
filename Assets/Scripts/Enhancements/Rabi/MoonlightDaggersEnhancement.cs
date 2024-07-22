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
                return "Rabi's weapon, pierces through enemies and deals damage.";
            case 2:
                return "Increases weapon damage by <color=\"green\">50%</color>";
            case 3:
                return "Increases projectiles lifetime and speed by <color=\"green\">25%</color>";
            case 4:
                return "Increases projectiles size by <color=\"green\">25%</color>";
            case 5:
                return "Shoot <color=\"green\">4</color> projectiles instead of 2.";
            case 6:
                return "Rabi's weapon, pierces through enemies and deals damage.";
            case 7:
                return "Increases weapon damage by <color=\"green\">50%</color>";
            case 8:
                return "Increases projectiles lifetime and speed by <color=\"green\">25%</color>";
            case 9:
                return "Increases weapon damage by <color=\"green\">50%</color>";
        }
    }

    public override string getId()
    {
        return "rabi.moonlightdaggers";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("attack.moonlightdaggers.level")) return 1;
        else return (int)Player.instance.abilityValues["attack.moonlightdaggers.level"];
    }

    public override string getName()
    {
        return "Moonlight Daggers";
    }

    public override string getType()
    {
        return "Attack";
    }

    public override int getWeight()
    {
        return 4;
    }

    public override bool isAvailable()
    {
        return Player.instance.abilityValues["attack.moonlightdaggers.level"] < 9;
    }

    public override bool isUnique()
    {
        return false;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.moonlightDaggers;
    }

    public override void OnEquip()
    {
        Player.instance.enhancements.Add(new MoonlightDaggersEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("attack.moonlightdaggers.level"))
        {
            Player.instance.abilityValues.Add("attack.moonlightdaggers.level", 1);
            UIManager.Instance.PlayerUI.SetWeaponIcon(IconList.instance.moonlightDaggers, 1, false);
        }
        else
        {
            Player.instance.abilityValues["attack.moonlightdaggers.level"] += 1;
            int level = (int)Player.instance.abilityValues["attack.moonlightdaggers.level"];
            UIManager.Instance.PlayerUI.SetWeaponLevel(level, level >= 10);
        }

        int lv = getLevel();

        Player.instance.abilityValues["Attack_Number"] = lv < 5 ? 2 : 4;
        Player.instance.abilityValues["Attack_Size"] = lv < 8 ? lv < 3 ? 1 : 1.25f : 1.5f;
        Player.instance.abilityValues["Attack_Damage"] = lv < 9 ? lv < 6 ? lv < 2 ? 12 : 18 : 24 : 30;
        Player.instance.abilityValues["Attack_Velocity"] = lv < 7 ? lv < 3 ? 1 : 1.25f : 1.5f;
        Player.instance.abilityValues["Attack_Time"] = lv < 7 ? lv < 3 ? 1 : 1.25f : 1.5f;

        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}