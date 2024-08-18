using JetBrains.Annotations;
using UnityEngine;

public class LunarPulseAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Rabi emits a pulse with the power of the moon that damages enemies every <color=\"green\">4</color> beats";
        }
        if (level == 2)
        {
            return "Lunar Pulses are <color=\"green\">25%</color> bigger and emit every <color=\"green\">4->3</color> beat";
        }
        if (level == 3)
        {
            return "Emits a pulse every <color=\"green\">3->2</color> Beats and knocks enemy back";
        }
        if (level == 4)
        {
            return "Emits every pulse and increases damage by <color=\"green\">100%</color>";
        }
        return "";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.lunarpulse.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.lunarpulse.level"];
    }

    public override string getName()
    {
        return "Lunar Pulse";
    }

    public override string getId()
    {
        return "rabi.lunarpulse";
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
        if(Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.lunarpulse") != null)
        {
            if (Player.instance.abilityValues["ability.lunarpulse.level"] < 4) available = true;
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
        return IconList.instance.lunarPulse;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new LunarPulseAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.lunarpulse.level"))
        {
            Player.instance.abilityValues.Add("ability.lunarpulse.level", 1);
            Player.instance.equippedPassiveAbilities.Add(new LunarPulseAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.lunarPulse, 1,false, Player.instance.getPassiveAbilityIndex(typeof(LunarPulseAbility)));
        }
        else
        {
            Player.instance.abilityValues["ability.lunarpulse.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.lunarpulse.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 4, Player.instance.getPassiveAbilityIndex(typeof(LunarPulseAbility)));
        }
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}