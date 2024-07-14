using UnityEngine;

public class StatEXPPercentEnhancement : Enhancement
{
    public const float expPercent = 0.1f;

    public override string GetDescription()
    {
        return "Increases Experience gained by <color=\"green\">10%</color>.";
    }

    public override string getId()
    {
        return "statExpPercentUp";
    }

    public override int getPriority()
    {
        return 0;
    }

    public override float getRarity()
    {
        return 30;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.expMultiUp;
    }

    public override bool isAvailable()
    {
        return true;
    }

    public override bool isUnique()
    {
        return false;
    }

    public override int getLevel()
    {
        return 0;
    }

    public override string getName()
    {
        return "Exp. Multi";
    }

    public override string getType()
    {
        return "Stat Up";
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveEnhancement(this);
        Player.instance.enhancements.Add(new StatEXPPercentEnhancement());
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        percentBonus.ExpMulti += expPercent;
    }
}