using JetBrains.Annotations;
using UnityEngine;

public class OrbitalMoonAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "<color=\"green\">2</color> moons orbit Rabi that deal damage and have a chance to block projectiles";
            case 2: return "Increase damage by <color=\"green\">20%</color>";
            case 3: return "Spins <color=\"green\">25%</color> faster and orbits for<color=\"green\">6->12</color> beats";
            case 4: return "Increase damage by <color=\"green\">30%</color>";
            case 5: return "Adds a <color=\"green\">third</color> moon";
            case 6: return "Moons have a <color=\"green\">10->25%</color> chance to destroy projectiles and are <color=\"green\">50%</color> bigger";
            case 7: return "Add small knockback on hit and never disappears";
        }
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.orbitalmoon.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.orbitalmoon.level"];
    }

    public override string getName()
    {
        return "Orbital Moon";
    }

    public override string getId()
    {
        return "rabi.orbitalmoon";
    }

    public override string getType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 3;
    }

    public override bool isAvailable()
    {
        bool available = true;
        if (Player.instance.equippedPassiveAbilities.Count == 5) available = false;
        if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.orbitalmoon") != null)
        {
            if (Player.instance.abilityValues["ability.orbitalmoon.level"] < 7) available = true;
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
        return IconList.instance.orbitalMoon;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new OrbitalMoonAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.orbitalmoon.level"))
        {
            Player.instance.abilityValues.Add("ability.orbitalmoon.level", 1);
            Player.instance.equippedPassiveAbilities.Add(new OrbitalMoonAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.orbitalMoon, 1,false, Player.instance.getPassiveAbilityIndex(typeof(OrbitalMoonAbility)));
        }
        else
        {
            Player.instance.abilityValues["ability.orbitalmoon.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.orbitalmoon.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 7, Player.instance.getPassiveAbilityIndex(typeof(OrbitalMoonAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}