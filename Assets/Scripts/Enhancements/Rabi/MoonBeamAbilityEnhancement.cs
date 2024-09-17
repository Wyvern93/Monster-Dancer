using JetBrains.Annotations;
using UnityEngine;

public class MoonBeamAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Shoot a moonlight ball of lasers that deal damage to enemies";
            case 2: return "Increase damage by <color=\"green\">20%</color>";
            case 3: return "Increase duration by <color=\"green\">50%</color>";
            case 4: return "Increase damage by <color=\"green\">30%</color>";
            case 5: return "Adds a <color=\"green\">second</color> moonlight ball";
            case 6: return "Increase size by <color=\"green\">25%</color>";
            case 7: return "Moonball casts moon echos when hitting enemies once per beat";
        }
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
        return 2;
    }

    public override bool isAvailable()
    {
        bool available = true;
        if (Player.instance.equippedPassiveAbilities.Count == 5) available = false;
        if (Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.moonbeam") != null)
        {
            if (Player.instance.abilityValues["ability.moonbeam.level"] < 7) available = true;
            else available = false;
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
            Player.instance.equippedPassiveAbilities.Add(new MoonBeamAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.moonBeam, 1, false, Player.instance.getPassiveAbilityIndex(typeof(MoonBeamAbility)));
        }
        else
        {
            Player.instance.abilityValues["ability.moonbeam.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.moonbeam.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 4, Player.instance.getPassiveAbilityIndex(typeof(MoonBeamAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}