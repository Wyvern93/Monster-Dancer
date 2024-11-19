using JetBrains.Annotations;
using UnityEngine;

public class MoonlightDaggersEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "Shoots two waves of moon energy towards the cursor";
    }

    public override string getId()
    {
        return "moonlightdaggers";
    }

    public override string getName()
    {
        return "Moonlight Daggers";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 4;
    }

    public override void OnEquip()
    {
        base.OnEquip();

        Player.instance.abilityValues["Attack_Number"] = 2;
        Player.instance.abilityValues["Attack_Size"] = 1f;
        Player.instance.abilityValues["Attack_Damage"] = 12f;
        Player.instance.abilityValues["Attack_Velocity"] = 1;
        Player.instance.abilityValues["Attack_Time"] = 0.25f;
        Player.instance.abilityValues["Attack_Cooldown"] = 2f;
        Player.instance.abilityValues["Attack_Pierce"] = 9999;

        Player.instance.CalculateStats();
    }

    public override PlayerAbility getAbility()
    {
        return new MoonlightDaggersAbility();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.Ability;
    }
}