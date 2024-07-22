using JetBrains.Annotations;
using UnityEngine;

public class BunnyHopAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Rabi can jump over tiles and obstacles, leaving behind a perfectly identical clone that lasts for <color=\"yellow\">20</color> beats.";
        }
        if (level == 2)
        {
            return "Reduces the cooldown from <color=\"yellow\">20->16</color> beats.";
        }
        if (level == 3)
        {
            return "Reduces the cooldown from <color=\"yellow\">16->12</color> beats.";
        }
        if (level == 4)
        {
            return "Rabi's perfectly identical clone lasts <color=\"yellow\">12->20</color> beats.";
        }
        return "";
    }

    public override string getId()
    {
        return "rabi.bunnyhop";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.bunnyhop.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.bunnyhop.level"];
    }

    public override string getName()
    {
        return "Bunny Hop";
    }

    public override string getType()
    {
        return "Active";
    }

    public override int getWeight()
    {
        return 3;
    }

    public override bool isAvailable()
    {
        bool available = false;
        if (Player.instance.activeAbility == null) available = true;
        else
        {
            if (Player.instance.activeAbility.getID() == "rabi.bunnyhop")
            {
                if (Player.instance.abilityValues["ability.bunnyhop.level"] < 4) available = true;
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
        return IconList.instance.bunnyhop;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new BunnyHopAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.bunnyhop.level"))
        {
            Player.instance.abilityValues.Add("ability.bunnyhop.level", 1);
            Player.instance.activeAbility = new BunnyHopAbility();
            UIManager.Instance.PlayerUI.SetActiveIcon(IconList.instance.bunnyhop, 1, false);
        }
        else
        {
            Player.instance.abilityValues["ability.bunnyhop.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.bunnyhop.level"];
            UIManager.Instance.PlayerUI.SetActiveLevel(level, level >= 4);
        }
        
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}