using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FireworksKitItem : PlayerItem
{
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new FireworksKitItemEnhancement() };
    }

    public override string getId()
    {
        return "fireworkskit";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>When an enemy is hit by an explosion, trigger a firework explosion</color>\n\n";
        description += AddStat("Explosion Damage", 25, true, "%");
        description += AddStat("Atk Speed", 25f, false, "%");
        description += AddStat("Cooldown", 25f, false, "%");

        return description;
    }

    public override string getItemName()
    {
        return "Fireworks Kit";
    }

    public override int getRarity()
    {
        return 1;
    }

    public override void OnCast()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEquip(PlayerAbility ability, int slot)
    {
        base.OnEquip(ability, slot);
        ability.itemValues["explosionDamage"] += 0.25f;
        ability.itemValues["atkSpeedMultiplier"] += 0.25f;
        ability.itemValues["cooldownMultiplier"] += 0.25f;
    }

    public override void OnUnequip(PlayerAbility ability, int originalSlot)
    {
        base.OnUnequip(ability, originalSlot);
        ability.itemValues["explosionDamage"] -= 0.25f;
        ability.itemValues["atkSpeedMultiplier"] -= 0.25f;
        ability.itemValues["cooldownMultiplier"] -= 0.25f;
    }

    public override void OnUpdate()
    {
    }

    public override void OnHit(PlayerAbility source, float damage, Enemy target)
    {
        if (source is IPlayerExplosion)
        {
            FireworkExplosion explosion = PoolManager.Get<FireworkExplosion>();
            explosion.abilitySource = source;

            float explosionSize = source.itemValues["explosionSize"];
            float explosionDamage = source.itemValues["explosionDamage"];
            if (source.hasItem(typeof(DetonationCatalystItem)))
            {
                explosionDamage *= explosionSize;
            }
            explosion.dmg = Mathf.Clamp(2f * source.itemValues["damageMultiplier"] * explosionDamage, 1, 10000);
            explosion.canSpawnMini = true;
            explosion.transform.position = target.transform.position;
            explosion.transform.localScale = Vector3.one * explosionSize * 0.5f;
        }
    }
}
