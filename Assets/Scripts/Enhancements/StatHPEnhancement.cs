using UnityEngine;

public class StatHPEnhancement : Enhancement
{
    public const int hpBonus = 10;

    public override string GetDescription()
    {
        return "Increases Max Health by <color=\"green\">10</color>.";
    }

    public override string getId()
    {
        return "statHPup";
    }

    public override int getWeight()
    {
        return 3;
    }

    public override bool isAvailable()
    {
        return true;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.hpUp;
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
        return "Health";
    }

    public override string getType()
    {
        return "Stat Up";
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveStatEnhancement(this);
        Player.instance.enhancements.Add(new StatHPEnhancement());
        Player.instance.CalculateStats();
        Player.instance.CurrentHP += hpBonus;
        UIManager.Instance.PlayerUI.UpdateHealth();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        flatBonus.MaxHP += hpBonus;
    }
}