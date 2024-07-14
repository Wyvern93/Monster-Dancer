using UnityEngine;

public class StatDMGEnhancement : Enhancement
{
    public const int dmgBonus = 5;

    public override string GetDescription()
    {
        return "Increases Damage by <color=\"green\">5%</color>.";
    }

    public override string getId()
    {
        return "statDMGup";
    }

    public override int getPriority()
    {
        return 0;
    }

    public override float getRarity()
    {
        return 20;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.dmgUp;
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
        return "Attack Damage";
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
        if (isUnique()) GameManager.runData.RemoveEnhancement(this);
        Player.instance.enhancements.Add(new StatDMGEnhancement());
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        percentBonus.Atk += 0.05f;
    }
}