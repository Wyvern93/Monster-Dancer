using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoonBeamAbility : PlayerAbility
{
    public int minCooldown = 1;
    private float xscale;

    public MoonBeamAbility() : base()
    {
        baseAmmo = 1;
        baseAttackSpeed = 0.5f;
        baseCooldown = 6;

        baseDamage = 60;
        baseCritChance = 0;
        baseDuration = 0.25f;
        baseSize = 1f;

        currentAmmo = GetMaxAmmo();
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
    }

    public override BeatManager.BeatType getBeatTrigger()
    {
        return BeatManager.BeatType.Compass;
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Shoots a powerful lunar beam across the screen that damages enemies in a burst</color>\n\n";
        description += AddStat("Uses", baseAmmo, GetMaxAmmo(), true);
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Attack Speed", baseAttackSpeed, GetAttackSpeed(), false, " Beats");
        description += AddStat("Damage", baseDamage, GetDamage(), true);
        description += AddStat("Crit Chance", baseCritChance, GetCritChance(), true, "%");
        description += AddStat("Duration", baseDuration, GetDuration(), true);
        description += AddStat("Size", baseSize * 0.6f, GetSize() * 0.6f, true);
        description += $"\nEvolves with: {starColor}{getEvolutionStarType()} Star";

        return description;
    }

    public override string getTags()
    {
        return "Tags: AOE, Lunar, Spin";
    }

    public override Color GetTooltipColor()
    {
        return new Color(0, 0.5f, 1f);
    }

    public override bool CanCast()
    {
        return BeatManager.GetBeatSuccess(BeatManager.BeatType.Compass) != BeatTrigger.FAIL;
    }

    public override string getAbilityName()
    {
        return "Moon Beam";
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new MoonBeamAbilityEnhancement() };
    }

    public override string getId()
    {
        return "moonbeam";
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override void OnCast()
    {/*
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
        UIManager.Instance.PlayerUI.SetAmmo(currentAmmo, GetMaxAmmo());*/
        MoonbeamLaser laser = PoolManager.Get<MoonbeamLaser>();
        laser.abilitySource = this;
        laser.abilityDamage = GetDamage();
        laser.Init(GetDuration(), GetSize(), GetTargetPosition(laser.mask));
        PlayerCamera.TriggerCameraShake(0.6f, 0.4f);
    }

    private Vector3 GetTargetPosition(LayerMask mask)
    {
        Vector2 target = Vector2.zero;
        float maxEnemies = 0;
        float angleDiff = 360f / 180f;

        for (int i = 0; i < 180; i++)
        {
            float angle = i * angleDiff;
            int around = Physics2D.RaycastAll(Player.instance.transform.position, BulletBase.angleToVector(angle), 12, mask).Length;

            if (around > maxEnemies)
            {
                target = BulletBase.angleToVector(angle);
                maxEnemies = around;
            }
        }
        //Debug.Log(maxEnemies);
        return target;
    }

    public override Sprite GetReloadIcon()
    {
        return IconList.instance.getReloadIcon("rabi_lunar");
    }

    public override Color GetRechargeColor()
    {
        return new Color(0.5f, 1f, 1f);
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