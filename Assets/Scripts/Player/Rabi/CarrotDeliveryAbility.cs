using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotDeliveryAbility : PlayerAbility, IPlayerProjectile
{
    public int minCooldown = 1;

    public override bool CanCast()
    {
        return Player.instance.CurrentSP >= Player.instance.MaxSP;
    }

    public override string getAbilityDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string getAbilityName()
    {
        return Localization.GetLocalizedString("ability.rabi.carrotdelivery.name");
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
        Player.instance.CurrentSP = 0;
        UIManager.Instance.PlayerUI.UpdateSpecial();
        Player.instance.StartCoroutine(UltimateCast());
    }

    public IEnumerator UltimateCast()
    {
        Camera cam = Camera.main;
        PlayerRabi rabi = (PlayerRabi)Player.instance;
        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        rabi.animator.Play("Rabi_Ultimate");

        AudioController.PlaySound(AudioController.instance.sounds.playerSpecialUseSfx);
        PlayerCamera.TriggerCameraShake(3f, 1f);
        CarrotDeliveryTruck truck = PoolManager.Get<CarrotDeliveryTruck>();
        truck.ability = this;
        Player.instance.despawneables.Add(truck);

        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        rabi.animator.Play("Rabi_Idle");
        yield break;
    }
    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}