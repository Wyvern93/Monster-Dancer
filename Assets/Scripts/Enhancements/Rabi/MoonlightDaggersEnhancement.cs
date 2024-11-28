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