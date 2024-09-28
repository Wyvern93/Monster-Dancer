using JetBrains.Annotations;
using UnityEngine;

public class PiercingShotAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Charge a powerful spinning carrot that pierces through enemies at high speed in the direction you are aiming";
            case 2: return "Increases damage by 30%";
            case 3: return "Reduces the cooldown by <color=\"green\">25%</color>";
            case 4: return "Increases damage by 30%";
            case 5: return "Increase size by <color=\"green\">50%</color>";
            case 6: return "Enemies hit are knocked back slightly";
            case 7: return "Splits into smaller drills that deal <color=\"green\">75%</color> damage when hitting enemies";
        }
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.piercingshot.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.piercingshot.level"];
    }

    public override string getName()
    {
        return "Piercing Shot";
    }

    public override string getId()
    {
        return "rabi.piercingshot";
    }

    public override string getType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 3; // How likely to appear
    }

    public override bool isAvailable()
    {
        bool available = true;
        if (Player.instance.equippedPassiveAbilities.Count == 5) available = false;

        if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.piercingshot") != null)
        {
            if (Player.instance.abilityValues["ability.piercingshot.level"] < 7) available = true;
            else available = false;
        }


        return available;
    }

    public override bool isUnique()
    {
        return false; // Unused with the exception of attacks
    }

    public override Sprite getIcon()
    {
        return IconList.instance.piercingShot;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new PiercingShotAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.piercingshot.level"))
        {
            Player.instance.abilityValues.Add("ability.piercingshot.level", 1);
            Player.instance.equippedPassiveAbilities.Add(new PiercingShotAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.piercingShot, 1,false, Player.instance.getPassiveAbilityIndex(typeof(PiercingShotAbility)));
        }
        else
        {
            Player.instance.abilityValues["ability.piercingshot.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.piercingshot.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 7, Player.instance.getPassiveAbilityIndex(typeof(PiercingShotAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus) // Unused for now
    {
        
    }
}