using JetBrains.Annotations;
using UnityEngine;

public class IllusionDashAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Controllable <color=\"green\">invulnerable</color> dash that dealing damage around and destroys projectiles";
        }
        if (level == 2)
        {
            return "Increase damage by <color=\"green\">25%</color>";
        }
        if (level == 3)
        {
            return "Decrease the cooldown by <color=\"green\">20%</color>";
        }
        if (level == 4)
        {
            return "Increase speed by <color=\"green\">25%</color>";
        }
        if (level == 5)
        {
            return "Increase damage by <color=\"green\">33%</color>";
        }
        if (level == 6)
        {
            return "Decrease the cooldown by <color=\"green\">25%</color>";
        }
        if (level == 7)
        {
            return "Knock back enemies and last <color=\"green\">66%</color> more time";
        }
        return "";
    }

    public override string getId()
    {
        return "rabi.illusiondash";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.illusiondash.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.illusiondash.level"];
    }

    public override string getName()
    {
        return "Illusion Dash";
    }

    public override string getType()
    {
        return "Active";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override bool isAvailable()
    {
        bool available = false;
        if (Player.instance.activeAbility == null) available = true;
        else
        {
            if (Player.instance.activeAbility.getID() == "rabi.illusiondash")
            {
                if (Player.instance.abilityValues["ability.illusiondash.level"] < 7) available = true;
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
        return IconList.instance.illusionDash;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new IllusionDashAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.illusiondash.level"))
        {
            Player.instance.abilityValues.Add("ability.illusiondash.level", 1);
            Player.instance.activeAbility = new IllusionDashAbility();
            UIManager.Instance.PlayerUI.SetActiveIcon(IconList.instance.illusionDash, 1, false);
        }
        else
        {
            Player.instance.abilityValues["ability.illusiondash.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.illusiondash.level"];
            UIManager.Instance.PlayerUI.SetActiveLevel(level, level >= 4);
        }
        
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}