using JetBrains.Annotations;
using UnityEngine;

public class CarrotJuiceAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Rabi throws a carrot juice bottle that spills and deals <color=\"green\">25%</color> of your damage repeatedly on contact";
        }
        if (level == 2)
        {
            return "Throws carrot bottles every <color=\"green\">20->12</color> beats and deals <color=\"green\">50%</color> more damage";
        }
        if (level == 3)
        {
            return "Carrot juice is <color=\"green\">200%</color> bigger";
        }
        if (level == 4)
        {
            return "Rabi throws <color=\"green\">1->2</color> carrot juice bottles, increases damage a <color=\"green\">50%</color>";
        }
        return "";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.carrotjuice.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.carrotjuice.level"];
    }

    public override string getName()
    {
        return "Carrot Juice";
    }

    public override string getId()
    {
        return "rabi.carrotjuice";
    }

    public override string getType()
    {
        return "Passive";
    }
    public override int getWeight()
    {
        return 4;
    }

    public override bool isAvailable()
    {
        bool available = true;
        if (Player.instance.equippedPassiveAbilities.Count == 3) available = false;
        if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.carrotjuice") != null)
        {
            if (Player.instance.abilityValues["ability.carrotjuice.level"] < 4) available = true;
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
        return IconList.instance.carrotJuice;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new CarrotJuiceAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.carrotjuice.level"))
        {
            Player.instance.abilityValues.Add("ability.carrotjuice.level", 1);
            Player.instance.equippedPassiveAbilities.Add(new CarrotJuiceAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.carrotJuice, 1,false, Player.instance.getPassiveAbilityIndex(typeof(CarrotJuiceAbility)));
        }
        else
        {
            Player.instance.abilityValues["ability.carrotjuice.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.carrotjuice.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 4, Player.instance.getPassiveAbilityIndex(typeof(CarrotJuiceAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}