using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunarPulseAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;
    public LunarPulseAbility() : base(2)
    {
    }

    public override bool CanCast()
    {
        return currentCooldown == 0;
    }

    public override string getAbilityDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string getAbilityName()
    {
        return Localization.GetLocalizedString("ability.rabi.lunarpulse.name");
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.lunarPulse;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new LunarPulseAbilityEnhancement() };
    }

    public override string getID()
    {
        return "rabi.lunarpulse";
    }

    public override void OnCast()
    {
        Player.instance.StartCoroutine(CastCoroutine());
    }

    public IEnumerator CastCoroutine()
    {
        level = (int)Player.instance.abilityValues["ability.lunarpulse.level"];

        maxCooldown = level < 2 ? 2 : 1;

        currentCooldown = maxCooldown;

        PlayerRabi rabi = (PlayerRabi)Player.instance;

        int numPulses = level >= 4 ? 4 : level < 4 ? 2 : 1;

        for (int i = 0; i < numPulses; i++)
        {
            LunarPulse pulse = PoolManager.Get<LunarPulse>();
            pulse.transform.position = rabi.transform.position;
            pulse.transform.parent = rabi.transform;
            yield return new WaitForSeconds(BeatManager.GetBeatDuration() / numPulses);
        }
        
    }

    public override void OnEquip()
    {

    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}