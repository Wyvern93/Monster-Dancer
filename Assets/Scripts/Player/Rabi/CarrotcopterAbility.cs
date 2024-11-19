using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotcopterAbility : PlayerAbility, IPlayerProjectile
{
    CarrotCopter currentDrone;
    public CarrotcopterAbility()
    {
        maxAmmo = 4;
        maxAttackSpeedCD = 0f;
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
        return Localization.GetLocalizedString("ability.rabi.carrotcopter.name");
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
        currentDrone.ForceDespawn(true);
    }

    public override void OnCast()
    {
        maxAmmo = 4;

        bool canShoot = currentDrone.CanShoot();
        if (!canShoot) return;

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
        currentDrone.Shoot();
        PlayerCamera.TriggerCameraShake(0.4f, 0.15f);
    }

    private void SpawnDrone()
    {
        CarrotCopter carrotcopter = PoolManager.Get<CarrotCopter>();
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