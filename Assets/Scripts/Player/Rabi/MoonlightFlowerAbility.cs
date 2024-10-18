using System.Collections.Generic;

using UnityEngine;

public class MoonlightFlowerAbility : PlayerAbility
{
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
        return Localization.GetLocalizedString("ability.rabi.moonlightflower.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new MoonlightFlowerAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "moonlightflower";
    }

    public override void OnCast()
    {

        int level = (int)Player.instance.abilityValues["ability.moonlightflower.level"];
        maxCooldown = level < 6 ? 20 : 15;
        
        currentCooldown = maxCooldown;

        int projectiles = level < 5 ? 5 : 8;
        for (int i = 0; i < projectiles; i++)
        {
            float angle = (360f / projectiles) * i;
            SpawnFlower(angle);
        }
    }

    public void SpawnFlower(float angle)
    {
        MoonlightFlower moon = PoolManager.Get<MoonlightFlower>();
        Player.instance.despawneables.Add(moon);
        moon.angle = angle;
        moon.OnSpawn();
    }

    public override void OnEquip()
    {

    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}