using UnityEngine;

public class StatDMGEnhancement : Enhancement
{
    public const int dmgBonus = 10;

    public override string GetDescription()
    {
        return "Increases Damage by <color=\"green\">10%</color>.";
    }

    public override string getId()
    {
        return "statDMGup";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.dmgUp;
    }
    public override bool isAvailable()
    {
        return true;
    }

    public override string getName()
    {
        return "Attack Damage";
    }

    public override string getDescriptionType()
    {
        return "Stat Up";
    }

    public override void OnEquip()
    {
        Player.instance.enhancements.Add(new StatDMGEnhancement());
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        percentBonus.Atk += 0.1f;
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Stat;
    }
}