using System.Collections.Generic;

using UnityEngine;

public class OrbitalMoonAbility : PlayerAbility
{

    public OrbitalMoonAbility() : base(20)
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
        return Localization.GetLocalizedString("ability.rabi.orbitalmoon.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new OrbitalMoonAbilityEnhancement() };
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.orbitalMoon;
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getID()
    {
        return "rabi.orbitalmoon";
    }

    public override void OnCast()
    {

        int level = (int)Player.instance.abilityValues["ability.orbitalmoon.level"];
        if (level >= 7)
        {
            maxCooldown = -100;
        }
        else
        {
            maxCooldown = 20;
        }
        
        currentCooldown = maxCooldown;

        int projectiles = level < 5 ? 2 : 3;
        for (int i = 0; i < projectiles; i++)
        {
            float angle = (360f / projectiles) * i;
            SpawnMoon(angle);
        }
    }

    public void SpawnMoon(float angle)
    {
        OrbitalMoon moon = PoolManager.Get<OrbitalMoon>();
        Player.instance.despawneables.Add(moon);
        moon.angle = angle;
    }

    public override void OnEquip()
    {

    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}