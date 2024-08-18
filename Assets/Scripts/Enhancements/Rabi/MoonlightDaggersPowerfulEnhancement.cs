using JetBrains.Annotations;
using UnityEngine;

public class MoonlightDaggersPowerfulEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Shorter waves that explode on contact with enemies dealing <color=\"green\">50%</color> of the weapon's damage";
    }

    public override string getId()
    {
        return "rabi.moonlightdaggers.powerful";
    }

    public override int getLevel()
    {
        return 6;
    }

    public override string getName()
    {
        return "Moonlight Daggers Powerful";
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
        Player.instance.enhancements.Add(new MoonlightDaggersPowerfulEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("attack.moonlightdaggers.level"))
        {
            Player.instance.abilityValues.Add("attack.moonlightdaggers.level", 1);
            UIManager.Instance.PlayerUI.SetWeaponIcon(IconList.instance.moonlightDaggers, 1, false);
        }
        else
        {
            Player.instance.abilityValues["attack.moonlightdaggers.level"] += 1;
            Player.instance.abilityValues.Add("attack.moonlightdaggers.awakening", 2);
            int level = (int)Player.instance.abilityValues["attack.moonlightdaggers.level"];
            UIManager.Instance.PlayerUI.SetWeaponLevel(level, true);

            Player.instance.abilityValues["Attack_Number"] = 2f;
            Player.instance.abilityValues["Attack_Size"] = 1.2f;
            Player.instance.abilityValues["Attack_Time"] = 0.75f;
            Player.instance.abilityValues["Attack_Damage"] = 17f;
            Player.instance.abilityValues["Attack_Cooldown"] = 1;
            Player.instance.abilityValues["Attack_Explode"] = 1;
        }
        
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}