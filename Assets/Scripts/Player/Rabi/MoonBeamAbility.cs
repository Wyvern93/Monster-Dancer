using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MoonBeamAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;
    public MoonBeamAbility() : base(0)
    {
    }

    public override bool CanCast()
    {
        Debug.Log(Player.instance.CurrentSP >= Player.instance.MaxSP);
        return Player.instance.CurrentSP >= Player.instance.MaxSP;
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

    public override string getID()
    {
        return "rabi.moonbeam";
    }

    public override void OnCast()
    {
        level = (int)Player.instance.abilityValues["ability.moonbeam.level"];
        Player.instance.CurrentSP = 0;
        UIManager.Instance.PlayerUI.UpdateSpecial();
        Player.instance.StartCoroutine(UltimateCast());
    }

    public IEnumerator UltimateCast()
    {
        Camera cam = Camera.main;
        PlayerRabi rabi = (PlayerRabi)Player.instance;
        rabi.isCastingMoonBeam = true;
        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration());
        rabi.animator.Play("Rabi_Ultimate");
        rabi.isMoving = true;
        MoonBeam moonBeam = PoolManager.Get<MoonBeam>();
        moonBeam.transform.position = new Vector3(rabi.transform.position.x, rabi.transform.position.y + 1.5f, 10f);
        moonBeam.transform.localEulerAngles = new Vector3(0, 0, 45f);

        while (moonBeam.isActiveAndEnabled) yield return new WaitForEndOfFrame();

        rabi.isMoving = false;
        rabi.animator.SetFloat("animatorSpeed", 1f / BeatManager.GetBeatDuration() / 2);
        rabi.animator.Play("Rabi_Idle");
        rabi.isCastingMoonBeam = false;
        yield break;
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.moonBeam;
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}