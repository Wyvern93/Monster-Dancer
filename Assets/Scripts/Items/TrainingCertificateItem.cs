using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingCertificateItem : PlayerItem
{
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new TrainingCertificateItemEnhancement() };
    }

    public override string getId()
    {
        return "trainingcertificate";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>Increases damage, ability size and duration but reduces the uses by 1</color>\n\n";
        description += AddStat("Damage", +20f, true, "%");
        description += AddStat("Ability Size", 25f, true, "%");
        description += AddStat("Ability Duration", 25f, true, "%");
        description += AddStat("Uses", -1, true, "");

        return description;
    }

    public override string getItemName()
    {
        return "Training Certificate";
    }

    public override int getRarity()
    {
        return 2;
    }

    public override void OnCast()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEquip(PlayerAbility ability, int slot)
    {
        base.OnEquip(ability, slot);
        ability.itemValues["damageMultiplier"] += 0.20f;
        ability.itemValues["sizeMultiplier"] += 0.25f;
        ability.itemValues["durationMultiplier"] += 0.25f;
        ability.itemValues["ammoExtra"] -= 1;
        if (ability.currentAmmo > ability.GetMaxAmmo()) ability.currentAmmo = ability.GetMaxAmmo();
    }

    public override void OnUnequip(PlayerAbility ability, int originalSlot)
    {
        base.OnUnequip(ability, originalSlot);
        ability.itemValues["damageMultiplier"] -= 0.20f;
        ability.itemValues["sizeMultiplier"] -= 0.20f;
        ability.itemValues["durationMultiplier"] -= 0.25f;
        ability.itemValues["ammoExtra"] += 1;
    }

    public override void OnUpdate()
    {

    }

    public override void OnHit(PlayerAbility source, float damage, Enemy target, bool isCritical)
    {
        /*
        if (source is IPlayerAura)
        {
        }
        */
    }
}
