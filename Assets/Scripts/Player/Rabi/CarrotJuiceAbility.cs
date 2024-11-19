using System.Collections.Generic;
using UnityEngine;

public class CarrotJuiceAbility : PlayerAbility
{
    public CarrotJuiceAbility()
    {
        maxAmmo = 3;
        maxAttackSpeedCD = 2f;
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
        return Localization.GetLocalizedString("ability.rabi.carrotjuice.name");
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
        int level = (int)Player.instance.abilityValues["ability.carrotjuice.level"];
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
        return typeof(BlessedFigureItem);
    }
}