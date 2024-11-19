using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;

public class MoonBeamAbility : PlayerAbility
{
    public int minCooldown = 1;

    public MoonBeamAbility()
    {
        maxAmmo = 1;
        maxAttackSpeedCD = 0f;
        maxCooldown = 8;

        currentAmmo = maxAmmo;
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
    }

    public override bool CanCast()
    {
        return currentCooldown == 0 && currentAttackSpeedCD == 0;
    }

    public override string getAbilityDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string getAbilityName()
    {
        return Localization.GetLocalizedString("ability.rabi.moonbeam.name");
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
    {
        if (currentAmmo - 1 > 0)
        {
            currentAmmo--;
            currentAttackSpeedCD = maxAttackSpeedCD;
        }
        else
        {
            currentAmmo--;
            currentCooldown = maxCooldown;
            currentAttackSpeedCD = maxAttackSpeedCD;
            AudioController.PlaySound(AudioController.instance.sounds.reloadSfx);
        }
        UIManager.Instance.PlayerUI.SetAmmo(currentAmmo, maxAmmo);
        MoonbeamLaser laser = PoolManager.Get<MoonbeamLaser>();
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
                currentAmmo = maxAmmo;
            }
        }
        if (BeatManager.isQuarterBeat && currentAttackSpeedCD > 0) currentAttackSpeedCD -= 0.25f;
        if (IsCurrentWeaponSelected())
        {
            UIManager.Instance.PlayerUI.SetAmmo(currentAmmo, maxAmmo);
        }
    }
}