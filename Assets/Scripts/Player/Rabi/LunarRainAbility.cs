using System.Collections.Generic;
using UnityEngine;

public class LunarRainAbility : PlayerAbility
{
    public LunarRainAbility(): base(4)
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
        return Localization.GetLocalizedString("ability.rabi.lunarrain.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new LunarRainAbilityEnhancement() };
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.lunarRain;
    }

    public override string getID()
    {
        return "rabi.lunarrain";
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.lunarrain.level"];
        maxCooldown = level < 5 ? level < 5 ? 4 : 2 : 1;
        currentCooldown = maxCooldown;

        CastRay();
        if (level >= 6) CastRay();
    }

    public void CastRay()
    {
        Enemy e = Map.GetRandomClosebyEnemy();
        if (e == null) return;

        LunarRainRay ray = PoolManager.Get<LunarRainRay>();
        ray.transform.position = e.transform.position;
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && !GameManager.isPaused && currentCooldown > 0) currentCooldown--;
    }
}