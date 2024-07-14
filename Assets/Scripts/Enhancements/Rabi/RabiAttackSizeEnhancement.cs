using UnityEngine;

public class RabiAttackSizeEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Increase the size of projectiles by <color=\"green\">10%</color>.";
    }

    public override string getId()
    {
        return "rabiAttackSizeUp";
    }

    public override int getPriority()
    {
        return 1;
    }

    public override float getRarity()
    {
        return 15;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.moonlightDaggers;
    }

    public override bool isAvailable()
    {
        return Player.instance.abilityValues["Attack_Size"] < Player.instance.abilityValues["Max_Attack_Size"];
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
        return "Moonlight Daggers";
    }

    public override string getType()
    {
        return "Attack";
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveEnhancement(this);
        Player.instance.enhancements.Add(new RabiAttackSizeEnhancement());
        Player.instance.abilityValues["Attack_Size"] += 0.1f;
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {
        
    }
}