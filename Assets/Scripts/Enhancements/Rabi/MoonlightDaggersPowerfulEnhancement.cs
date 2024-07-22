using JetBrains.Annotations;
using UnityEngine;

public class MoonlightDaggersPowerfulEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Rabi awakens her Moonlight Daggers, shooting big projectiles that deal high amounts of damage";
    }

    public override string getId()
    {
        return "rabi.moonlightdaggers.powerful";
    }

    public override int getLevel()
    {
        return 9;
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
        return 1;
    }

    public override bool isAvailable()
    {
        if (!Player.instance.abilityValues.ContainsKey("attack.moonlightdaggers.level")) return false;
        return Player.instance.abilityValues["attack.moonlightdaggers.level"] == 9;
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

            Player.instance.abilityValues["Attack_Number"] = 4f;
            Player.instance.abilityValues["Attack_Size"] = 3f;
            Player.instance.abilityValues["Attack_Damage"] = 24f;
        }
        
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}