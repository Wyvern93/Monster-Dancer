using UnityEngine;

public class BonusCoinsEnhancement : Enhancement
{
    int coins;
    public override string GetDescription()
    {
        coins = Random.Range(150, 600);
        return $"+<color=\"green\">{coins}</color> Monster Coins";
    }

    public override string getId()
    {
        return "bonusCoins";
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
        return IconList.instance.coins;
    }

    public override int getLevel()
    {
        return 0;
    }

    public override string getName()
    {
        return "Monster Coins";
    }

    public override string getDescriptionType()
    {
        return "Bonus";
    }

    public override void OnEquip()
    {
        Player.instance.enhancements.Add(new BonusCoinsEnhancement());

        GameManager.runData.coins += coins;
        UIManager.Instance.PlayerUI.coinText.text = GameManager.runData.coins.ToString();

        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Stat;
    }
}