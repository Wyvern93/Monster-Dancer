using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EclipseAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;

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
        return Localization.GetLocalizedString("ability.rabi.eclipse.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new EclipseAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return true;
    }
    public override string getId()
    {
        return "eclipse";
    }

    public override void OnCast()
    {
        level = (int)Player.instance.abilityValues["ability.eclipse.level"];
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


        RabiEclipse eclipse = PoolManager.Get<RabiEclipse>();
        Player.instance.despawneables.Add(eclipse);

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