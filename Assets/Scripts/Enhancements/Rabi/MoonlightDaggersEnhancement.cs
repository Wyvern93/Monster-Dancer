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
                return "Shoots two waves of moon energy in front";
            case 2:
                return "Increases weapon damage by <color=\"green\">20%</color>";
            case 3:
                return "Attacks last <color=\"green\">0.25s->0.5s</color>";
            case 4:
                return "Reduce the time between attacks by <color=\"green\">33%</color>";
            case 5:
                return "Increase size by <color=\"green\">30%</color>";
            case 6:
                return "Increases weapon damage by <color=\"green\">20%</color>";
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
        return Player.instance.abilityValues["attack.moonlightdaggers.level"] < 6;
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
            UIManager.Instance.PlayerUI.SetWeaponLevel(level, level >= 7);
        }

        int lv = getLevel();

        /*
        case 1:
            return "Shoots two waves of moon energy in front";
        case 2:
            return "Increases weapon damage by <color=\"green\">20%</color>";
        case 3:
            return "Attacks last <color=\"green\">0.5s->1.5s</color>";
        case 4:
            return "Reduce the time between attacks by <color=\"green\">33%</color>";
        case 5:
            return "Increase size by <color=\"green\">20%</color>";
        case 6:
            return "Increases weapon damage by <color=\"green\">20%</color>";*/

        Player.instance.abilityValues["Attack_Number"] = 2;
        Player.instance.abilityValues["Attack_Size"] = lv < 5 ? 1f : 1.3f;
        Player.instance.abilityValues["Attack_Damage"] = lv < 6 ? lv < 2 ? 12 : 14 : 17;
        Player.instance.abilityValues["Attack_Velocity"] = 1;
        Player.instance.abilityValues["Attack_Time"] = lv < 3 ? 0.25f : 0.5f;
        Player.instance.abilityValues["Attack_Cooldown"] = lv < 4 ? 2 : 1;
        Player.instance.abilityValues["Attack_Pierce"] = 9999;

        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}