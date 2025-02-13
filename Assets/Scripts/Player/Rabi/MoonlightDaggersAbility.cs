using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonlightDaggersAbility : PlayerAbility, IPlayerProjectile
{
    public MoonlightDaggersAbility() : base()
    {
        baseAmmo = 2;
        baseAttackSpeed = 0.25f;
        baseCooldown = 2;
        baseDamage = 12;
        baseDuration = 0.5f;
        baseSpeed = 12;
        baseKnockback = 1;
        baseCritChance = 0;

        currentAmmo = GetMaxAmmo();
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
    }

    public override bool CanCast()
    {
        return currentCooldown == 0 && currentAttackSpeedCD == 0;
    }

    public override Color GetTooltipColor()
    {
        return new Color(0, 0.5f, 1f);
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Shoots two waves of moonlight energy that pierce and pushes through enemies</color>\n\n";
        description += AddStat("Uses", baseAmmo, GetMaxAmmo(), true);
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Attack Speed", baseAttackSpeed, GetAttackSpeed(), false, " Beats");
        description += AddStat("Damage per Wave", baseDamage, GetDamage(), true);
        description += AddStat("Crit Chance", baseCritChance, GetCritChance(), true, "%");
        description += AddStat("Speed", baseSpeed, GetSpeed(), true);
        description += AddStat("Duration", baseDuration, GetDuration(), true, " Beats");
        description += AddStat("Knockback", baseKnockback, GetKnockback(), true);
        description += $"\nEvolves with: {starColor}{getEvolutionStarType()} Star";

        return description;
    }

    public override string getTags()
    {
        return "Tags: Projectile, Piercing, Lunar, Knockback";
    }

    public override string getAbilityName()
    {
        return "Moonlight Daggers";
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new MoonlightDaggersEnhancement() }; // Used for the menu
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "moonlightdaggers";
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
        ShootWave();
    }

    public IEnumerator CastCoroutine(int level)
    {
        ShootWave();

        float time = BeatManager.GetBeatDuration() * 0.25f;

        while (time > 0)
        {
            while (GameManager.isPaused) yield return null;
            time -= Time.deltaTime;
            yield return null;
        }
        ShootWave();
        yield break;
    }

    public override Sprite GetReloadIcon()
    {
        return IconList.instance.getReloadIcon("rabi_lunar");
    }

    public override Color GetRechargeColor()
    {
        return new Color(0.5f, 1f, 1f);
    }

    public void ShootWave()
    {
        MoonlightDaggerWave wave = PoolManager.Get<MoonlightDaggerWave>();
        wave.quarterbeats = GetDuration();
        wave.velocity = GetSpeed();
        wave.dmg = GetDamage();
        wave.abilitySource = this;
        PlayerCamera.TriggerCameraShake(0.25f, 0.15f);
        Player.instance.despawneables.Add(wave.GetComponent<IDespawneable>());
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

    public override Enhancement getEvolutionEnhancement()
    {
        return new FlamingDrillAbilityEnhancement();
    }
}