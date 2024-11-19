using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarrotBarrageAbility : PlayerAbility
{
    private List<ExplosiveCarrot> carrots = new List<ExplosiveCarrot>();
    
    public CarrotBarrageAbility()
    {
        maxAmmo = 3;
        maxAttackSpeedCD = 3f;
        maxCooldown = 6;

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
        return Localization.GetLocalizedString("ability.rabi.carrotbarrage.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotBarrageAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "carrotbarrage";
    }

    public override Color GetRechargeColor()
    {
        return new Color(1f, 0.5f, 0f);
    }

    public override void OnCast()
    {
        carrots.Clear();
        
        maxCooldown = 4;
        maxAmmo = 3;
        maxAttackSpeedCD = 2f;
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

        float dmg = 15f;
        int numberOfCarrots = 3;

        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;

        float baseAngle = BulletBase.VectorToAngle(difference);
        float perCarrotAngle = (10 / (numberOfCarrots / 2));
        baseAngle -= 10;

        float distanceToCursor = (crosshairPos - (Vector2)Player.instance.transform.position).magnitude;
        distanceToCursor = Mathf.Clamp(distanceToCursor, 0, 7);

        for (int i = 0; i < numberOfCarrots; i++) 
        {
            float angle = baseAngle + (perCarrotAngle * i);

            // Calculate distance multiplier: center carrots go farther than those on the sides
            float distanceMultiplier = 1f - Mathf.Abs((i - (numberOfCarrots - 1) / 2f) / (numberOfCarrots / 2f));
            float carrotDistance = distanceToCursor * (0.5f + distanceMultiplier * 0.5f);

            // Determine direction and distance for each carrot
            Vector2 dir = BulletBase.angleToVector(angle).normalized * carrotDistance;
            CastCarrot(dir, dmg, i == 0);
        }
        AudioController.PlaySound((Player.instance as PlayerRabi).throwSound);
    }



    public void CastCarrot(Vector2 direction, float damage, bool playSound)
    {
        ExplosiveCarrot carrot = PoolManager.Get<ExplosiveCarrot>();
        carrot.transform.position = Player.instance.transform.position;
        carrot.isSmall = false;
        carrot.Init(direction);
        carrot.dmg = damage;
        
        Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());
    }

    public override Sprite GetReloadIcon()
    {
        return IconList.instance.getReloadIcon("rabi_carrot");
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

    public override Enhancement getEvolutionEnhancement()
    {
        return new ExplosiveFestivalAbilityEnhancement();
    }
}