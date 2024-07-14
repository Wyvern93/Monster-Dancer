using UnityEngine;

public class RabiAttackVelocityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Increases projectiles' velocity by <color=\"green\">20%</color>.";
    }

    public override string getId()
    {
        return "rabiAttackVelocityUp";
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
        return Player.instance.abilityValues["Attack_Velocity"] < Player.instance.abilityValues["Max_Attack_Velocity"];
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
        Player.instance.abilityValues["Attack_Velocity"] += 0.2f;
        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}