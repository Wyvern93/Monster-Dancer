using System.Collections.Generic;
using UnityEngine;

public class MoonlightFlowerAbility : PlayerAbility
{
    public float baseShockwaveDamage;

    public List<MoonlightFlower> currentFlowers;
    public MoonlightFlowerAbility() : base()
    {
        baseAmmo = 1;
        baseAttackSpeed = 0f;
        baseCooldown = 8;

        baseDamage = 4;
        baseShockwaveDamage = 12;
        baseKnockback = 0;
        baseCritChance = 0;
        baseDuration = 6;
        baseSpeed = 4;
        baseSize = 1f;

        currentAmmo = GetMaxAmmo();
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
        currentFlowers = new List<MoonlightFlower>();
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Shoots a flower of moon energy that moves towards the cursor, dealing damage every 1/4 Beats in an area.\n\nSends a shockwave upon despawning</color>\n\n";
        description += AddStat("Uses", baseAmmo, GetMaxAmmo(), true);
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Attack Speed", baseAttackSpeed, GetAttackSpeed(), false, " Beats");
        description += AddStat("Damage", baseDamage, GetDamage(), true);
        description += AddStat("Shockwave Damage", baseShockwaveDamage, GetShockwaveDamage(), true);
        description += AddStat("Crit Chance", baseCritChance * 100, GetCritChance() * 100, true, "%");
        description += AddStat("Size", baseSize * 3f, GetSize() * 3f, true);
        description += $"\nEvolves with: {starColor}{getEvolutionStarType()} Star";

        return description;
    }

    public override Color GetTooltipColor()
    {
        return new Color(0, 0.5f, 1f);
    }

    public override string getTags()
    {
        return "Tags: AOE, Lunar, Spin";
    }

    public override float GetDamage()
    {
        float rawDmg = baseDamage * itemValues["damageMultiplier"] * itemValues["orbitalDamage"];
        return Mathf.Clamp(rawDmg, 1, 1000);
    }

    public override float GetSpeed()
    {
        float rawSpeed = baseSpeed * itemValues["speedMultiplier"] * itemValues["orbitalSpeed"];
        return Mathf.Clamp(rawSpeed, 1, 1000);
    }

    public float GetShockwaveDamage()
    {
        return Mathf.Clamp(baseShockwaveDamage * itemValues["damageMultiplier"], 0, 1000);
    }

    public override bool CanCast()
    {
        return currentCooldown == 0 && currentAttackSpeedCD == 0;
    }

    public override string getAbilityName()
    {
        return "Moonlight Flower";
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new MoonlightFlowerAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "moonlightflower";
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
        SpawnFlower();
        PlayerCamera.TriggerCameraShake(0.6f, 0.4f);
    }

    public override Sprite GetReloadIcon()
    {
        return IconList.instance.getReloadIcon("rabi_lunar");
    }

    public override Color GetRechargeColor()
    {
        return new Color(0.5f, 1f, 1f);
    }

    public void SpawnFlower()
    {
        MoonlightFlower moon = PoolManager.Get<MoonlightFlower>();
        moon.abilitySource = this;
        moon.dmg = GetDamage();
        moon.speed = GetSpeed();
        moon.beats = GetDuration();
        moon.targetScale = GetSize();
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;

        if (difference == Vector2.zero) difference = Vector2.right;

        currentFlowers.Add(moon);
        moon.transform.position = Player.instance.Sprite.transform.position + ((Vector3)difference) + new Vector3(0, 0, 3);
        Player.instance.despawneables.Add(moon);
        moon.OnSpawn();
    }

    public override void OnChange()
    {
        base.OnChange();
        List<MoonlightFlower> list = currentFlowers;
        foreach (MoonlightFlower flower in list)
        {
            if (flower != null)
            {
                if (!flower.gameObject.activeSelf) currentFlowers.Remove(flower);
                else flower.ForceDespawn(false);
            }
        }
        
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