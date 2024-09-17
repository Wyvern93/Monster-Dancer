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
    {/*
        case 1: return "<color=\"green\">2</color> moons orbit Rabi that deal damage and have a chance to block projectiles";
        case 2: return "Increase damage by <color=\"green\">20%</color>";
        case 3: return "Spins <color=\"green\">25%</color> faster and orbits for<color=\"green\">2s->5s</color>";
        case 4: return "Increase damage by <color=\"green\">30%</color>";
        case 5: return "Adds a <color=\"green\">third</color> moon";
        case 6: return "Moons have a <color=\"green\">10->25%</color> chance to destroy projectiles";
        case 7:
            return "Add small knockback on hit";
        */

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