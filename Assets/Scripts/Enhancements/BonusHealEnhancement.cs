using UnityEngine;

public class BonusHealEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return $"Heals <color=\"green\">30</color> HP";
    }

    public override string getId()
    {
        return "bonusHeal";
    }

    public override int getWeight()
    {
        return 1;
    }

    public override bool isAvailable()
    {
        return true;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.heal;
    }

    public override string getName()
    {
        return "Heal";
    }

    public override string getDescriptionType()
    {
        return "Bonus";
    }

    public override void OnEquip()
    {
        Player.instance.enhancements.Add(new BonusHealEnhancement());

        Player.instance.CurrentHP = (int)Mathf.Clamp(Player.instance.CurrentHP + 30, 0, Player.instance.currentStats.MaxHP);
        UIManager.Instance.PlayerUI.coinText.text = GameManager.runData.coins.ToString();

        Player.instance.CalculateStats();
        UIManager.Instance.PlayerUI.UpdateHealth();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Stat;
    }
}