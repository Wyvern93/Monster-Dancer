using JetBrains.Annotations;
using UnityEngine;

public class CarrotBarrageAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Rabi throws <color=\"green\">4</color> carrots that explode after a second";
        }
        if (level == 2)
        {
            return "Rabi throws <color=\"green\">4->6</color> carrots";
        }
        if (level == 3)
        {
            return "Rabi throws <color=\"green\">6->8</color> carrots";
        }
        if (level == 4)
        {
            return "Rabi throws <color=\"green\">8->12</color> carrots";
        }
        return "";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.carrotbarrage.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.carrotbarrage.level"];
    }

    public override string getName()
    {
        return "Carrot Barrage";
    }

    public override string getId()
    {
        return "rabi.carrotbarrage";
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

        if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.carrotbarrage") != null)
        {
            if (Player.instance.abilityValues["ability.carrotbarrage.level"] < 4) available = true;
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
        return IconList.instance.carrotBarrage;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new CarrotBarrageAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.carrotbarrage.level"))
        {
            Player.instance.abilityValues.Add("ability.carrotbarrage.level", 1);
            Player.instance.equippedPassiveAbilities.Add(new CarrotBarrageAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.carrotBarrage, 1,false, Player.instance.getPassiveAbilityIndex(typeof(CarrotBarrageAbility)));
        }
        else
        {
            Player.instance.abilityValues["ability.carrotbarrage.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.carrotbarrage.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 4, Player.instance.getPassiveAbilityIndex(typeof(CarrotBarrageAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}