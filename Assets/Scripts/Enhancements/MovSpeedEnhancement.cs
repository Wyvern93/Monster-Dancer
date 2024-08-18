using UnityEngine;

public class MovSpeedEnhancement : Enhancement
{

    public override string GetDescription()
    {
        return "Increases Movement Speed by <color=\"green\">5%</color>.";
    }

    public override string getId()
    {
        return "statMovSpeedup";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.movSpeedUp;
    }

    public override bool isAvailable()
    {
        return true;
    }

    public override int getLevel()
    {
        return 0;
    }

    public override string getName()
    {
        return "Mov Speed";
    }

    public override string getType()
    {
        return "Stat Up";
    }

    public override bool isUnique()
    {
        return false;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveStatEnhancement(this);
        Player.instance.enhancements.Add(new MovSpeedEnhancement());
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        flatBonus.Speed += 0.05f;
    }
}