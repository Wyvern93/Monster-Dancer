using JetBrains.Annotations;
using UnityEngine;

public class MoonlightDaggersMasterfulEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Weapon attacks have now a spread and shoot really fast";
    }

    public override string getId()
    {
        return "rabi.moonlightdaggers.masterful";
    }

    public override int getLevel()
    {
        return 6;
    }

    public override string getName()
    {
        return "Moonlight Daggers Masterful";
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
        if (!Player.instance.abilityValues.ContainsKey("attack.moonlightdaggers.level")) return false;
        return Player.instance.abilityValues["attack.moonlightdaggers.level"] == 6;
    }

    public override bool isUnique()
    {
        return true;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.moonlightDaggers;
    }

    public override void OnEquip()
    {
        Player.instance.enhancements.Add(new MoonlightDaggersMasterfulEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("attack.moonlightdaggers.level"))
        {
            Player.instance.abilityValues.Add("attack.moonlightdaggers.level", 1);
            UIManager.Instance.PlayerUI.SetWeaponIcon(IconList.instance.moonlightDaggers, 1, false);
        }
        else
        {
            Player.instance.abilityValues["attack.moonlightdaggers.level"] += 1;
            int level = (int)Player.instance.abilityValues["attack.moonlightdaggers.level"];
            Player.instance.abilityValues.Add("attack.moonlightdaggers.awakening", 1);
            UIManager.Instance.PlayerUI.SetWeaponLevel(level, true);

            Player.instance.abilityValues["Attack_Number"] = 4f;
            Player.instance.abilityValues["Attack_Velocity"] = 1.5f;
            Player.instance.abilityValues["Attack_Time"] = 1f;
            Player.instance.abilityValues["Attack_Damage"] = 12f;
            Player.instance.abilityValues["Attack_Cooldown"] = 0;
            Player.instance.abilityValues["Attack_Spread"] = 25f * Mathf.Deg2Rad;
        }
        
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}