using System.Collections.Generic;
using UnityEngine;

public class CarrotJuiceAbility : PlayerAbility
{
    public float baseSplashDamage;
    float juiceSizeBase = 2.93f;
    float splashSize = 0.9f;
    float baseSlow = 0.5f;

    public float GetSplashDamage()
    {
        return Mathf.Clamp(baseSplashDamage * itemValues["damageMultiplier"], 0, 1000);
    }

    public float GetSlow()
    {
        return Mathf.Clamp(baseSlow * itemValues["slowMultiplier"], 0, 100);
    }

    public CarrotJuiceAbility() : base()
    {
        baseAmmo = 3;
        baseAttackSpeed = 2f;
        baseCooldown = 8;

        baseDamage = 4;
        baseSplashDamage = 8;
        baseDuration = 12;
        baseSpeed = 8;
        baseCritChance = 0;
        baseReach = 6;
        baseSlow = 0.5f;
        baseSize = 1;

        currentAmmo = GetMaxAmmo();
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Throw a bottle of carrot juice that splashes on enemies and leaves juice on the ground, dealing damage and slowing enemies</color>\n\n";
        description += AddStat("Uses", baseAmmo, GetMaxAmmo(), true);
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Attack Speed", baseAttackSpeed, GetAttackSpeed(), false, " Beats");
        description += AddStat("Splash Damage", baseSplashDamage, GetSplashDamage(), true);
        description += AddStat("Juice Damage", baseDamage, GetDamage(), true);
        description += AddStat("Crit Chance", baseCritChance * 100, GetCritChance() * 100, true, "%");
        description += AddStat("Slowness", baseSlow * 100, GetSlow() * 100, true, "%");
        description += AddStat("Juice Duration", baseDuration, GetDuration(), true, " Beats");
        description += AddStat("Splash Size", splashSize, splashSize * GetSize(), true);
        description += AddStat("Juice Size", juiceSizeBase, juiceSizeBase * GetSize(), true);
        description += AddStat("Reach", baseReach, GetReach(), true);
        description += AddStat("Throw Speed", baseSpeed, GetSpeed(), true);
        description += $"\nEvolves with: {starColor}{getEvolutionStarType()} Star";

        return description;
    }

    public override string getTags()
    {
        return "Tags: Projectile, AOE, Debuff, Carrot, Falling";
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
        return "Carrot Juice";
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotJuiceAbilityEnhancement() };
    }

    public override string getId()
    {
        return "carrotjuice";
    }
    public override bool isUltimate()
    {
        return false;
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

        AudioController.PlaySound((Player.instance as PlayerRabi).throwSound);

        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;
        float distanceToCursor = (crosshairPos - (Vector2)Player.instance.transform.position).magnitude;
        distanceToCursor = Mathf.Clamp(distanceToCursor, 0, 6);

        CastBottle(difference * distanceToCursor);
    }

    public void CastBottle(Vector2 direction)
    {
        CarrotJuiceBottle bottle = PoolManager.Get<CarrotJuiceBottle>();
        bottle.transform.position = Player.instance.transform.position;
        bottle.Init(direction);
        bottle.ability = this;
        Player.instance.despawneables.Add(bottle);
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