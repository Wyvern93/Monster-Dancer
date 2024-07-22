using JetBrains.Annotations;
using UnityEngine;

public class LunarRainAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Every <color=\"green\">4</color> beats a Lunar Beam falls on an enemy, dealing damage";
        }
        if (level == 2)
        {
            return "Lunar Beams fall every <color=\"green\">4->2</color> beats";
        }
        if (level == 3)
        {
            return "<color=\"green\">2</color> Lunar Beams fall every time";
        }
        if (level == 4)
        {
            return "When an enemy dies rom Lunar Rain, another Lunar Beam falls";
        }
        return "";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.lunarrain.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.lunarrain.level"];
    }

    public override string getName()
    {
        return "Lunar Rain";
    }

    public override string getId()
    {
        return "rabi.lunarrain";
    }

    public override string getType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override bool isAvailable()
    {
        bool available = true;
        if (Player.instance.equippedPassiveAbilities.Count == 3) available = false;
        else if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.lunarrain") != null)
        {
            if (Player.instance.abilityValues["ability.lunarrain.level"] >= 4) available = false;
        }


        return available;
    }

    public override bool isUnique()
    {
        return false;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.lunarRain;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new LunarRainAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.lunarrain.level"))
        {
            Player.instance.abilityValues.Add("ability.lunarrain.level", 1);
            Player.instance.equippedPassiveAbilities.Add(new LunarRainAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.lunarRain, 1,false, Player.instance.getPassiveAbilityIndex(typeof(LunarRainAbility)));
        }
        else
        {
            Player.instance.abilityValues["ability.lunarrain.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.lunarrain.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 4, Player.instance.getPassiveAbilityIndex(typeof(LunarRainAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}