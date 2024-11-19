using System.Collections.Generic;
using UnityEngine;

public class LunarRainAbility : PlayerAbility
{
    float range = 2;
    int numEnemies = 15;
    public LunarRainAbility()
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
        return Localization.GetLocalizedString("ability.rabi.lunarrain.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new LunarRainAbilityEnhancement() };
    }

    public override string getId()
    {
        return "lunarrain";
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.lunarrain.level"];
        //maxCooldown = level < 5 ? level < 3 ? 4 : 2 : 1;
        //currentCooldown = maxCooldown;

        //maxCooldown = 4;//level < 4 ? 3 : 2;
        //currentCooldown = maxCooldown;
        //maxAmmo = 3;
        //maxAttackSpeedCD = 2f;
        List<Enemy> enemies = getAllEnemies();
        if (enemies.Count <= 0) return;

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
        foreach (Enemy e in enemies)
        {
            CastRay(e);
        }
        PlayerCamera.TriggerCameraShake(0.6f, 0.2f);
    }

    public List<Enemy> getAllEnemies()
    {
        List<Enemy> enemies = new List<Enemy>();
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        foreach (Enemy enemy in Map.Instance.enemiesAlive)
        {
            if (enemy == null) continue;
            if (enemies.Count >= numEnemies) return enemies;
            if (Vector2.Distance(enemy.transform.position, crosshairPos) < range) enemies.Add(enemy);
        }

        return enemies;
    }

    public void CastRay(Enemy e)
    {
        LunarRainRay ray = PoolManager.Get<LunarRainRay>();
        ray.transform.position = e.transform.position;
    }

    public override Sprite GetReloadIcon()
    {
        return IconList.instance.getReloadIcon("rabi_lunar");
    }

    public override Color GetRechargeColor()
    {
        return new Color(0.5f, 1f, 1f);
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