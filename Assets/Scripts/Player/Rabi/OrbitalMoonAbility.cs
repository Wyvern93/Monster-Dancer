using System.Collections.Generic;
using UnityEditor.Playables;
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

    public override string getID()
    {
        return "rabi.orbitalmoon";
    }

    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.orbitalmoon.level"];
        maxCooldown = level < 2 ? 20 : 16;
        currentCooldown = maxCooldown;

        int projectiles = level < 4 ? 3 : 4;
        for (int i = 0; i < projectiles; i++)
        {
            float angle = (360f / projectiles) * i;
            SpawnMoon(angle);
        }
    }

    public void SpawnMoon(float angle)
    {
        OrbitalMoon moon = PoolManager.Get<OrbitalMoon>();
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