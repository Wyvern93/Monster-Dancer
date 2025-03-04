using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarrotDeliveryAbility : PlayerAbility, IPlayerProjectile
{
    public int baseRunOverDamage;

    public float GetRunOverDamage()
    {
        return Mathf.Clamp(baseRunOverDamage * itemValues["damageMultiplier"], 0, 1000);
    }
    public CarrotDeliveryAbility() : base()
    {
        baseCooldown = 180;
        baseDamage = 30;
        baseRunOverDamage = 150;
        baseSpeed = 10;

        currentCooldown = 0;
    }

    public override string getAbilityDescription()
    {
        string starColor = GetStarColorText();

        string description = $"<color=#FFFF88>Calls a delivery truck that does 4 laps running over and shooting enemies</color>\n\n";
        description += AddStat("Cooldown", baseCooldown, GetMaxCooldown(), false, " Beats");
        description += AddStat("Run Over Damage", baseRunOverDamage, GetRunOverDamage(), true);
        description += AddStat("Bullet Damage", baseDamage, GetDamage(), true);
        description += AddStat("Velocity", baseSpeed, GetSpeed(), true);

        return description;
    }

    public override Color GetTooltipColor()
    {
        return new Color(1, 0.5f, 0f);
    }

    public override bool CanCast()
    {
        return currentCooldown <= 0;
    }

    public override void OnChange()
    {
        
    }

    public override string getAbilityName()
    {
        return "Carrot Delivery";
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotDeliveryAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return true;
    }
    public override string getId()
    {
        return "carrotdelivery";
    }

    public override void OnCast()
    {
        currentCooldown = GetMaxCooldown();
        UIManager.Instance.PlayerUI.UpdateSpecial(currentCooldown, GetMaxCooldown());
        Player.instance.StartCoroutine(UltimateCast());
    }

    public IEnumerator UltimateCast()
    {
        PlayerRabi rabi = (PlayerRabi)Player.instance;
        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        rabi.animator.Play("Rabi_Ultimate");

        AudioController.PlaySound(AudioController.instance.sounds.playerSpecialUseSfx);
        PlayerCamera.TriggerCameraShake(3f, 1f);
        CarrotDeliveryTruck truck = PoolManager.Get<CarrotDeliveryTruck>();
        truck.ability = this;
        truck.truckDmg = GetRunOverDamage();
        truck.bulletDmg = GetDamage();
        truck.velocity = GetSpeed();
        Player.instance.despawneables.Add(truck);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        rabi.animator.Play("Rabi_Idle");
        yield break;
    }
    public override void OnEquip()
    {
        currentCooldown = GetMaxCooldown();
        UIManager.Instance.PlayerUI.UpdateSpecial(currentCooldown, GetMaxCooldown());
    }

    public override void OnUpdate()
    {
        if (BeatManager.isQuarterBeat && currentCooldown > 0) currentCooldown -= 0.25f;
        UIManager.Instance.PlayerUI.UpdateSpecial(currentCooldown, GetMaxCooldown());
    }
}