using JetBrains.Annotations;
using UnityEngine;

public class EclipseAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Summon an Eclipse that shoots 4 pulses that deal damage and stun enemies and bullets, heal 6% of your HP for every pulse";
            case 2: return "Lasts <color=\"green\">1</color> additional pulse";
            case 3: return "Pulses also deal <color=\"green\">5%</color> of the enemies health";
            case 4: return "Increase healing of each pulse by <color=\"green\">2%</color>";
            case 5: return "Lasts <color=\"green\">1</color> additional pulse";
            case 6: return "Increase healing of each pulse by <color=\"green\">2%</color>";
            case 7: return "Pulses now destroy bullets";
        }
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.eclipse.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.eclipse.level"];
    }

    public override string getName()
    {
        return "Eclipse";
    }

    public override string getType()
    {
        return "Special";
    }

    public override string getId()
    {
        return "rabi.eclipse";
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
            if (Player.instance.ultimateAbility.getID() == "rabi.eclipse")
            {
                if (Player.instance.abilityValues["ability.eclipse.level"] < 7) available = true;
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
        return IconList.instance.Eclipse;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new EclipseAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.eclipse.level"))
        {
            Player.instance.abilityValues.Add("ability.eclipse.level", 1);
            Player.instance.ultimateAbility = new EclipseAbility();
            Player.AddSP(250);
            UIManager.Instance.PlayerUI.ShowSPBar();
            UIManager.Instance.PlayerUI.SetUltimateIcon(IconList.instance.Eclipse, 1, false);
            UIManager.Instance.PlayerUI.UpdateSpecial();
        }
        else
        {
            if (getLevel() == 6) Player.instance.MaxSP = (int)(Player.instance.MaxSP * 0.75f);
            UIManager.Instance.PlayerUI.UpdateSpecial();
            Player.instance.abilityValues["ability.eclipse.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.eclipse.level"];
            UIManager.Instance.PlayerUI.SetUltimateLevel(level, level >= 7);
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}