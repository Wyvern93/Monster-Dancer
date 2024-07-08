public class StatHPEnhancement : Enhancement
{
    public const int hpBonus = 10;

    public override string getId()
    {
        return "statHPup";
    }

    public override void OnEquip()
    {
        if (unique) GameManager.runData.RemoveEnhancement(this);
        Player.instance.enhancements.Add(new StatHPEnhancement());
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        flatBonus.MaxHP += hpBonus;
    }
}