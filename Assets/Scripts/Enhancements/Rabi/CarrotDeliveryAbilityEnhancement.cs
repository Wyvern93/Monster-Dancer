using UnityEngine;

public class CarrotDeliveryAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        switch (level)
        {
            default:
            case 1: return "Call a Delivery Truck that runs overs enemies and shoots carrot bullets";
            case 2: return "Increases bullet damage from <color=\"green\">20 -> 35</color>";
            case 3: return "Reduces the cooldown from <color=\"green\">250 -> 225</color> beats";
            case 4: return "Increases bullet damage from <color=\"green\">35 -> 50</color>";
            case 5: return "The Delivery Truck runs <color=\"yellow\">4 -> 8</color> laps and <color=\"yellow\">50%</color> faster";
            case 6: return "Reduces the cooldown from <color=\"green\">225 -> 200</color> beats";
            case 7: return "The Delivery Truck also destroys bullets";
        }
    }

    public override string getName()
    {
        return "Carrot Delivery";
    }

    public override string getDescriptionType()
    {
        return "Special";
    }

    public override string getId()
    {
        return "carrotdelivery";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override PlayerAbility getAbility()
    {
        return new CarrotDeliveryAbility();
    }

    public override void OnEquip()
    {
        Player.instance.enhancements.Add(this);
        if (getLevel() == 0)
        {
            Player.instance.abilityValues.Add($"ability.{getId()}.level", 1);
            Player.instance.ultimateAbility = getAbility();
            Player.AddSP(250);
            UIManager.Instance.PlayerUI.ShowSPBar();
            UIManager.Instance.PlayerUI.SetUltimateIcon(getIcon(), 1, false);
            UIManager.Instance.PlayerUI.UpdateSpecial();
        }
        else
        {
            Player.instance.abilityValues[$"ability.{getId()}.level"] += 1;
            int level = getLevel();
            if (getLevel() == 3) Player.instance.SetMaxSP(225);
            else if (getLevel() == 6) Player.instance.SetMaxSP(200);
            UIManager.Instance.PlayerUI.UpdateSpecial();
            Player.instance.abilityValues[$"ability.{getId()}.level"] += 1;
            UIManager.Instance.PlayerUI.SetUltimateLevel(level, level >= 7);
        }
        Player.instance.CalculateStats();
    }

    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}