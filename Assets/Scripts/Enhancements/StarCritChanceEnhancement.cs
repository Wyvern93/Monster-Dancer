using UnityEngine;

public class StatCritChanceEnhancement : Enhancement
{

    public override string GetDescription()
    {
        return "Increases Crit Chance by <color=\"green\">10%</color>.";
    }

    public override string getId()
    {
        return "statCritChanceup";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.critChanceUp;
    }

    public override bool isAvailable()
    {
        return true;
    }

    public override string getName()
    {
        return "Crit Chance";
    }

    public override string getDescriptionType()
    {
        return "Stat Up";
    }

    public override void OnEquip()
    {
        Player.instance.enhancements.Add(new StatCritChanceEnhancement());
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        flatBonus.CritChance += 10f;
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Stat;
    }
}