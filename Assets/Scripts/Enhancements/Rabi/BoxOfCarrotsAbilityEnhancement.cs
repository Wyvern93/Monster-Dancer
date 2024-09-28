using JetBrains.Annotations;
using UnityEngine;

public class BoxOfCarrotsAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Boxes fall from the sky, healing the player on contact or exploding when touched by enemies";
            case 2: return "Increases damage by 30%";
            case 3: return "Reduces the cooldown by <color=\"green\">30%</color>";
            case 4: return "Increases damage by 30%";
            case 5: return "Increase healing by <color=\"green\">100%</color>";
            case 6: return "Increases damage by 30%";
            case 7: return "<color=\"green\">2</color> boxes fall every time";
        }
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.boxofcarrots.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.boxofcarrots.level"];
    }

    public override string getName()
    {
        return "Box of Carrots";
    }

    public override string getId()
    {
        return "rabi.boxofcarrots";
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

        if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.boxofcarrots") != null)
        {
            if (Player.instance.abilityValues["ability.boxofcarrots.level"] < 7) available = true;
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
        return IconList.instance.boxofCarrots;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new BoxOfCarrotsAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.boxofcarrots.level"))
        {
            Player.instance.abilityValues.Add("ability.boxofcarrots.level", 1);
            Player.instance.equippedPassiveAbilities.Add(new BoxOfCarrotsAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.boxofCarrots, 1,false, Player.instance.getPassiveAbilityIndex(typeof(BoxOfCarrotsAbility)));
        }
        else
        {
            Player.instance.abilityValues["ability.boxofcarrots.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.boxofcarrots.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 7, Player.instance.getPassiveAbilityIndex(typeof(BoxOfCarrotsAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}