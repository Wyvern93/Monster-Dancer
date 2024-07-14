using UnityEngine;

public class RabiAttackProjectilesEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Shoots an additional projectile";
    }

    public override string getId()
    {
        return "rabiAttackProjectilesUp";
    }

    public override int getPriority()
    {
        return 1;
    }

    public override float getRarity()
    {
        return 35;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.moonlightDaggers;
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

    public override bool isAvailable()
    {
        return Player.instance.abilityValues["Attack_Number"] < Player.instance.abilityValues["Max_Attack_Number"];
    }

    public override bool isUnique()
    {
        return false;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveEnhancement(this);
        Player.instance.enhancements.Add(new RabiAttackProjectilesEnhancement());
        Player.instance.abilityValues["Attack_Number"] += 1f;
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}