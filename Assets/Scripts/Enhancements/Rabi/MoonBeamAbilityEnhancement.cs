using JetBrains.Annotations;
using UnityEngine;

public class MoonBeamAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Rabi calls the power of the moon and projects it as three beams of moonlight that destroy projectiles and damage enemies for <color=\"yellow\">12</color> beats";
        }
        if (level == 2)
        {
            return "Increases the damage of the beams by <color=\"yellow\">25%</color>";
        }
        if (level == 3)
        {
            return "Increases the speed of the beams by <color=\"yellow\">25%</color>";
        }
        if (level == 4)
        {
            return "Increases the duration of the Moon Beams from <color=\"yellow\">12->18</color> beats and Rabi recovers <color=\"yellow\">2%</color> HP with every hit";
        }
        return "";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.moonbeam.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.moonbeam.level"];
    }

    public override string getName()
    {
        return "Moon Beam";
    }

    public override string getType()
    {
        return "Special";
    }

    public override string getId()
    {
        return "rabi.moonbeam";
    }

    public override int getWeight()
    {
        return 3;
    }

    public override bool isAvailable()
    {
        bool available = false;
        if (Player.instance.ultimateAbility == null) available = true;
        else
        {
            if (Player.instance.ultimateAbility.getID() == "rabi.moonbeam")
            {
                if (Player.instance.abilityValues["ability.moonbeam.level"] < 4) available = true;
            }
        }

        return available;
    }

    public override bool isUnique()
    {
        return false;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.moonBeam;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new MoonBeamAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.moonbeam.level"))
        {
            Player.instance.abilityValues.Add("ability.moonbeam.level", 1);
            Player.instance.ultimateAbility = new MoonBeamAbility();
            UIManager.Instance.PlayerUI.ShowSPBar();
            UIManager.Instance.PlayerUI.SetUltimateIcon(IconList.instance.moonBeam, 1, false);
        }
        else
        {
            Player.instance.abilityValues["ability.moonbeam.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.moonbeam.level"];
            UIManager.Instance.PlayerUI.SetUltimateLevel(level, level >= 4);
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}