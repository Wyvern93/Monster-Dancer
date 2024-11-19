using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;

public class MoonBeamAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;

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
        level = (int)Player.instance.abilityValues["ability.moonbeam.level"];
        //maxCooldown = 14; //level < 4 ? level < 3 ? level < 2 ? 4 : 3 : 2 : 1;

        //currentCooldown = maxCooldown;

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
        /*
        PlayerRabi rabi = (PlayerRabi)Player.instance;
        int beams = level < 5 ? 1 : 2;
        for (int i = 0; i < beams; i++)
        {
            MoonBeam moonBeam = PoolManager.Get<MoonBeam>();
            moonBeam.transform.position = new Vector3(rabi.transform.position.x, rabi.transform.position.y + 1.5f, 10f);
            moonBeam.transform.localEulerAngles = new Vector3(0, 0, 45f);
            Player.instance.despawneables.Add(moonBeam);

            Enemy e = Map.GetRandomClosebyEnemy(rabi.transform.position);
            if (e == null) moonBeam.movDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            else moonBeam.movDir = (e.transform.position - rabi.transform.position);
        }*/
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