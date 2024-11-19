using System.Collections.Generic;
using UnityEngine;

public class MoonlightFlowerAbility : PlayerAbility
{
    public List<MoonlightFlower> currentFlowers;
    public MoonlightFlowerAbility()
    {
        maxAmmo = 1;
        maxAttackSpeedCD = 0f;
        maxCooldown = 8;

        currentAmmo = maxAmmo;
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
        currentFlowers = new List<MoonlightFlower>();
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
        return Localization.GetLocalizedString("ability.rabi.moonlightflower.name");
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
        foreach (MoonlightFlower flower in currentFlowers)
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