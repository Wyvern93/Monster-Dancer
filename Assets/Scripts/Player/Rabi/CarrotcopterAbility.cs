using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotcopterAbility : PlayerAbility, IPlayerProjectile
{
    CarrotCopter currentDrone;
    public CarrotcopterAbility() : base()
    {
        baseAmmo = 4;
        baseAttackSpeed = 0.25f;
        baseCooldown = 4;

        baseDamage = 15;
        baseKnockback = 0;
        baseCritChance = 0;
        baseDuration = 0;
        baseSpeed = 7;
        baseReach = 5;
        baseSize = 5f;

        currentAmmo = GetMaxAmmo();
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>A drone that shoots targeted enemies in an area</color>\n\n";
        description += AddStat("Uses", baseAmmo, GetMaxAmmo(), true);
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Attack Speed", baseAttackSpeed, GetAttackSpeed(), false, " Beats");
        description += AddStat("Damage per Bullet", baseDamage, GetDamage(), true);
        description += AddStat("Crit Chance", baseCritChance * 100, GetCritChance() * 100, true, "%");
        description += AddStat("Drone Reach", baseReach, GetReach(), true);
        description += AddStat("Speed", baseSpeed, GetSpeed(), true);
        description += $"\nEvolves with: {starColor}{getEvolutionStarType()} Star";

        return description;
    }

    public override string getTags()
    {
        return "Tags: Projectile, Summon, Single-Target";
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
        return "Carrotcopter";
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotcopterAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "carrotcopter";
    }

    public override void OnSelect()
    {
        base.OnSelect();
        SpawnDrone();
    }

    public override void OnChange()
    {
        base.OnChange();
        if (currentDrone == null) return;
        currentDrone.ForceDespawn(true);
    }

    public override void OnCast()
    {
        if (currentDrone == null) return;
        bool canShoot = currentDrone.CanShoot();
        if (!canShoot) return;

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
        currentDrone.Shoot();
        PlayerCamera.TriggerCameraShake(0.4f, 0.15f);
    }

    private void SpawnDrone()
    {
        CarrotCopter carrotcopter = PoolManager.Get<CarrotCopter>();
        carrotcopter.abilitySource = this;
        carrotcopter.transform.position = Player.instance.transform.position;
        currentDrone = carrotcopter;
        Player.instance.despawneables.Add(carrotcopter);
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