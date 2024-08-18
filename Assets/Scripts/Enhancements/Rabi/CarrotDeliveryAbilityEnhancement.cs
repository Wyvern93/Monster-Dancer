using JetBrains.Annotations;
using UnityEngine;

public class CarrotDeliveryAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Rabi calls a Delivery Truck to assist her in battle, running over enemies and shooting carrots for <color=\"yellow\">4</color> laps";
        }
        if (level == 2)
        {
            return "The Delivery Truck runs <color=\"yellow\">4->8</color> laps and <color=\"yellow\">50%</color> faster";
        }
        if (level == 3)
        {
            return "Increases Truck and Carrot damage by <color=\"yellow\">50%</color>";
        }
        if (level == 4)
        {
            return "The Delivery Truck also destroys bullets";
        }
        return "";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.carrotdelivery.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.carrotdelivery.level"];
    }

    public override string getName()
    {
        return "Carrot Delivery";
    }

    public override string getType()
    {
        return "Special";
    }

    public override string getId()
    {
        return "rabi.carrotdelivery";
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
            if (Player.instance.ultimateAbility.getID() == "rabi.carrotdelivery")
            {
                if (Player.instance.abilityValues["ability.carrotdelivery.level"] < 4) available = true;
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
        return IconList.instance.carrotDelivery;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new CarrotDeliveryAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.carrotdelivery.level"))
        {
            Player.instance.abilityValues.Add("ability.carrotdelivery.level", 1);
            Player.instance.ultimateAbility = new CarrotDeliveryAbility();
            UIManager.Instance.PlayerUI.ShowSPBar();
            UIManager.Instance.PlayerUI.SetUltimateIcon(IconList.instance.carrotDelivery, 1, false);
        }
        else
        {
            Player.instance.abilityValues["ability.carrotdelivery.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.carrotdelivery.level"];
            UIManager.Instance.PlayerUI.SetUltimateLevel(level, level >= 4);
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}