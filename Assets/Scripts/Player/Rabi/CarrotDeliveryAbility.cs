using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CarrotDeliveryAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;
    public CarrotDeliveryAbility() : base(0)
    {
    }

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

    public override string getID()
    {
        return "rabi.carrotdelivery";
    }

    public override void OnCast()
    {
        level = (int)Player.instance.abilityValues["ability.carrotdelivery.level"];
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

        CarrotDeliveryTruck truck = PoolManager.Get<CarrotDeliveryTruck>();

        yield return new WaitForSeconds(0.2f);

        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 2);
        rabi.animator.Play("Rabi_Idle");
        yield break;
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.carrotDelivery;
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}