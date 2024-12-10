using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarrotBarrageAbility : PlayerAbility, IPlayerExplosion
{
    private List<ExplosiveCarrot> carrots = new List<ExplosiveCarrot>();

    public override float GetDamage() // Explosion Damage
    {
        float explosionSize = itemValues["explosionSize"];
        float explosionDamage = itemValues["explosionDamage"];
        if (hasItem(typeof(DetonationCatalystItem)))
        {
            explosionDamage *= explosionSize;
        }
        return Mathf.Clamp(baseDamage * itemValues["damageMultiplier"] * explosionDamage, 1, 10000);
    }

    public override float GetSize()
    {
        float rawSize = baseSize * itemValues["sizeMultiplier"] * itemValues["explosionSize"];
        return Mathf.Clamp(rawSize, 1, 100);
    }

    public CarrotBarrageAbility() : base()
    {
        baseAmmo = 3;
        baseAttackSpeed = 2f;
        baseCooldown = 6;
        baseDamage = 25;
        baseKnockback = 0;
        baseCritChance = 0;
        baseDuration = 2;
        baseSpeed = 8;
        baseReach = 7;
        baseSpread = 15;
        baseSize = 1.5f;

        currentAmmo = GetMaxAmmo();
        currentAttackSpeedCD = 0;
        currentCooldown = 0;
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Shoots three carrots in a cone into the air that explode after falling</color>\n\n";
        description += AddStat("Uses", baseAmmo, GetMaxAmmo(), true);
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Attack Speed", baseAttackSpeed, GetAttackSpeed(), false, " Beats");
        description += AddStat("Explosion Damage", baseDamage, GetDamage(), true);
        description += AddStat("Crit Chance", baseCritChance, GetCritChance(), true, "%");
        description += AddStat("Reach", baseReach, GetReach(), true);
        description += AddStat("Spread", baseSpread, GetSpread(), true);
        description += AddStat("Speed", baseSpeed, GetSpeed(), true);
        description += AddStat("Explosion Size", baseSize, GetSize(), true);
        description += $"\nEvolves with: {starColor}{getEvolutionStarType()} Star";

        return description;
    }

    public override string getTags()
    {
        return "Tags: Projectile, Explosion, Carrot, Falling";
    }

    public override Color GetTooltipColor()
    {
        return new Color(1, 0.5f, 0f);
    }

    public override bool CanCast()
    {
        return currentCooldown == 0 && currentAttackSpeedCD == 0;
    }

    public override string getAbilityName()
    {
        return "Carrot Barrage";
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
        UIManager.Instance.PlayerUI.SetAmmo(currentAmmo, GetMaxAmmo());

        float dmg = GetDamage();
        int numberOfCarrots = 3;

        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;

        float baseAngle = BulletBase.VectorToAngle(difference);
        float perCarrotAngle = (GetSpread() / (numberOfCarrots / 2));
        baseAngle -= GetSpread();

        float distanceToCursor = (crosshairPos - (Vector2)Player.instance.transform.position).magnitude;
        distanceToCursor = Mathf.Clamp(distanceToCursor, 0, GetReach());

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
        carrot.abilitySource = this;
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

    public override Enhancement getEvolutionEnhancement()
    {
        return new ExplosiveFestivalAbilityEnhancement();
    }
}