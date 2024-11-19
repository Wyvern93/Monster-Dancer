using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotBusterAbility : PlayerAbility
{
    public CarrotBusterAbility()
    {
        maxAmmo = 2;
        maxAttackSpeedCD = 1f;
        maxCooldown = 4;

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
        return Localization.GetLocalizedString("ability.rabi.carrotbuster.name");
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

        CarrotBuster carrotBuster = PoolManager.Get<CarrotBuster>();
        PlayerCamera.TriggerCameraShake(0.4f, 0.4f);
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
                currentAmmo = maxAmmo;
            }
        }
        if (BeatManager.isQuarterBeat && currentAttackSpeedCD > 0) currentAttackSpeedCD -= 0.25f;
        if (IsCurrentWeaponSelected())
        {
            UIManager.Instance.PlayerUI.SetAmmo(currentAmmo, maxAmmo);
        }
    }

    public override System.Type getEvolutionItemType()
    {
        return typeof(FireworksKitItem);
    }
}