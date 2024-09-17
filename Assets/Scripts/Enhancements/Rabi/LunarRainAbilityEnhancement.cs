using JetBrains.Annotations;
using UnityEngine;

public class LunarRainAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1) return "Lunar beams fall from the sky onto enemies closeby, dealing damage";
        if (level == 2) return "Increases damage by 30%";
        if (level == 3) return "Lunar Beams fall every <color=\"green\">4->2</color> beats";
        if (level == 4) return "Increase damage by 30%";
        if (level == 5) return "Lunar Beams fall every <color=\"green\">2->1</color> beat";
        if (level == 6) return "<color=\"green\">2</color> Lunar Beams fall every time";
        if (level == 7) return "When an enemy dies from Lunar Rain, another Lunar Beam falls";
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
        if (Player.instance.equippedPassiveAbilities.Count == 5) available = false;
        if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.lunarrain") != null)
        {
            if (Player.instance.abilityValues["ability.lunarrain.level"] < 7) available = true;
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
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 7, Player.instance.getPassiveAbilityIndex(typeof(LunarRainAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}