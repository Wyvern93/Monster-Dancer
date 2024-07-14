using UnityEngine;

public class RabiAttackTimeEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Increase the lifespan of projectiles by <color=\"green\">25%</color>.";
    }

    public override string getId()
    {
        return "rabiAttackTimeUp";
    }

    public override int getPriority()
    {
        return 1;
    }

    public override float getRarity()
    {
        return 20;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.moonlightDaggers;
    }

    public override bool isAvailable()
    {
        return Player.instance.abilityValues["Attack_Time"] < Player.instance.abilityValues["Max_Attack_Time"];
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
        Player.instance.enhancements.Add(new RabiAttackVelocityEnhancement());
        Player.instance.abilityValues["Attack_Time"] += 0.25f;
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}