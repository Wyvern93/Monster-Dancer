using JetBrains.Annotations;
using UnityEngine;

public class OrbitalMoonAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "<color=\"green\">3</color> moons orbit Rabi, dealing damage to enemies and some times blocking projectiles";
        }
        if (level == 2)
        {
            return "Moons have a chance of <color=\"green\">20->35%</color> of blocking projectiles and reduce the cooldown from <color=\"green\">20->16</color> beats";
        }
        if (level == 3)
        {
            return "Spins <color=\"green\">25%</color> faster and deals<color=\"green\">50%</color> more damage";
        }
        if (level == 4)
        {
            return "<color=\"green\">3->4</color> moons orbit Rabi";
        }
        return "";
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
        if (Player.instance.equippedPassiveAbilities.Count == 3) available = false;
        else if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.orbitalmoon") != null)
        {
            if (Player.instance.abilityValues["ability.orbitalmoon.level"] >= 4) available = false;
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
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 4, Player.instance.getPassiveAbilityIndex(typeof(OrbitalMoonAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}