using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotBusterAbility : PlayerAbility
{
    public CarrotBusterAbility() : base()
    {
        baseAmmo = 2;
        baseAttackSpeed = 1f;
        baseCooldown = 2;
        baseDamage = 25;
        baseDuration = 2;
        baseSize = 1f;
        baseKnockback = 5;
        baseCritChance = 0;

        currentAmmo = GetMaxAmmo();
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
    }
    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Swing a giant sword-shaped carrot that deals damage and pushes enemies</color>\n\n";
        description += AddStat("Uses", baseAmmo, GetMaxAmmo(), true);
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Attack Speed", baseAttackSpeed, GetAttackSpeed(), false, " Beats");
        description += AddStat("Damage", baseDamage, GetDamage(), true);
        description += AddStat("Crit Chance", baseCritChance, GetCritChance(), true, "%");
        description += AddStat("Size", baseSize * 3.5f, 3.5f * GetSize(), true);
        description += AddStat("Knockback", baseKnockback, GetKnockback(), true);
        description += $"\nEvolves with: {starColor}{getEvolutionStarType()} Star";

        return description;
    }

    public override string getTags()
    {
        return "Tags: Melee, Carrot, Knockback";
    }

    public override Color GetTooltipColor()
    {
        return new Color(1, 0.5f, 0f);
    }

    public override bool CanCast()
    {
        return currentCooldown == 0 && currentAttackSpeedCD == 0;
    }

    public override string getAbilityName()
    {
        return "Carrot Buster";
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotBusterAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "carrotbuster";
    }

    public override void OnCast()
    {
        if (currentAmmo - 1 > 0)
        {
            currentAmmo--;
            currentAttackSpeedCD = GetAttackSpeed();
        }
        else
        {
            currentAmmo--;
            currentCooldown = GetMaxCooldown();
            currentAttackSpeedCD = GetAttackSpeed();
            AudioController.PlaySound(AudioController.instance.sounds.reloadSfx);
        }
        UIManager.Instance.PlayerUI.SetAmmo(currentAmmo, GetMaxAmmo());

        CarrotBuster carrotBuster = PoolManager.Get<CarrotBuster>();
        carrotBuster.abilitySource = this;
        carrotBuster.damage = GetDamage();
        carrotBuster.transform.localScale = Vector3.one * GetSize();
        PlayerCamera.TriggerCameraShake(0.2f, 0.2f);
    }

    public override Sprite GetReloadIcon()
    {
        return IconList.instance.getReloadIcon("rabi_carrot");
    }

    public override Color GetRechargeColor()
    {
        return new Color(1f, 0.5f, 0f);
    }

    public override void OnEquip()
    {

    }

    public override void OnUpdate()
    {
        if (BeatManager.isQuarterBeat && currentCooldown > 0)
        {
            currentCooldown -= 0.25f;
            if (currentCooldown == 0)
            {
                currentAttackSpeedCD = 0;
                currentAmmo = GetMaxAmmo();
            }
        }
        if (BeatManager.isQuarterBeat && currentAttackSpeedCD > 0) currentAttackSpeedCD -= 0.25f;
        if (IsCurrentWeaponSelected())
        {
            UIManager.Instance.PlayerUI.SetAmmo(currentAmmo, GetMaxAmmo());
        }
    }
}