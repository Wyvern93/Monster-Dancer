using System.Collections.Generic;
using UnityEngine;

public class LunarRainAbility : PlayerAbility
{
    int numEnemies = 50;
    public LunarRainAbility() : base()
    {
        baseAmmo = 3;
        baseAttackSpeed = 2f;
        baseCooldown = 8;

        baseDamage = 20;
        baseKnockback = 0;
        baseCritChance = 0;
        baseDuration = 0;
        baseSpeed = 0;
        baseReach = 2;
        baseSpread = 0;
        baseSize = 1.0f;

        currentAmmo = GetMaxAmmo();
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Strike enemies in an area with falling beams of moon energy that deal damage</color>\n\n";
        description += AddStat("Uses", baseAmmo, GetMaxAmmo(), true);
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Attack Speed", baseAttackSpeed, GetAttackSpeed(), false, " Beats");
        description += AddStat("Damage per beam", baseDamage, GetDamage(), true);
        description += AddStat("Crit Chance", baseCritChance * 100, GetCritChance() * 100, true, "%");
        description += AddStat("Area Reach", baseReach, GetReach(), true);
        description += $"\nEvolves with: {starColor}{getEvolutionStarType()} Star";

        return description;
    }

    public override string getTags()
    {
        return "Tags: Beam, Lunar, Single-Target";
    }

    public override Color GetTooltipColor()
    {
        return new Color(0, 0.5f, 1f);
    }

    public override bool CanCast()
    {
        return currentCooldown == 0 && currentAttackSpeedCD == 0;
    }


    public override string getAbilityName()
    {
        return "Lunar Rain";
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new LunarRainAbilityEnhancement() };
    }

    public override string getId()
    {
        return "lunarrain";
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override void OnCast()
    {
        List<Enemy> enemies = getAllEnemies();
        if (enemies.Count <= 0) return;

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
        foreach (Enemy e in enemies)
        {
            CastRay(e);
        }
        PlayerCamera.TriggerCameraShake(0.6f, 0.2f);
    }

    public List<Enemy> getAllEnemies()
    {
        List<Enemy> enemies = new List<Enemy>();
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        foreach (Enemy enemy in Map.Instance.enemiesAlive)
        {
            if (enemy == null) continue;
            if (enemies.Count >= numEnemies) return enemies;
            if (Vector2.Distance(enemy.transform.position, crosshairPos) < GetReach()) enemies.Add(enemy);
        }

        return enemies;
    }

    public void CastRay(Enemy e)
    {
        LunarRainRay ray = PoolManager.Get<LunarRainRay>();
        ray.transform.position = e.transform.position;
        ray.abilitySource = this;
        ray.dmg = GetDamage();

        float damage = GetDamage();
        bool isCritical = GetCritChance() > Random.Range(0f, 100f);
        if (isCritical) damage *= 2.5f;

        e.TakeDamage((int)damage, isCritical);
        foreach (PlayerItem item in equippedItems)
        {
            if (item == null) continue;
            item.OnHit(this, GetDamage(), e);
        }
    }

    public override Sprite GetReloadIcon()
    {
        return IconList.instance.getReloadIcon("rabi_lunar");
    }

    public override Color GetRechargeColor()
    {
        return new Color(0.5f, 1f, 1f);
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