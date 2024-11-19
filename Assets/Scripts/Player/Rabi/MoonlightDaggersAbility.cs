using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonlightDaggersAbility : PlayerAbility, IPlayerProjectile
{
    public MoonlightDaggersAbility()
    {
        maxAmmo = 2;
        maxAttackSpeedCD = 0f;
        maxCooldown = 2;

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
        throw new System.NotImplementedException(); // This is unused since it's used the AbilityEnhancement
    }

    public override string getAbilityName()
    {
        return Localization.GetLocalizedString("ability.rabi.moonlightdaggers.name"); // Unused
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
        ShootWave();
    }

    public IEnumerator CastCoroutine(int level)
    {
        ShootWave();

        float time = BeatManager.GetBeatDuration() * 0.25f;

        while (time > 0)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
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
        PlayerCamera.TriggerCameraShake(0.4f, 0.15f);
        Player.instance.despawneables.Add(wave.GetComponent<IDespawneable>());
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
        return typeof(HotSauceBottleItem);
    }

    public override Enhancement getEvolutionEnhancement()
    {
        return new FlamingDrillAbilityEnhancement();
    }
}