using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;

public class MoonBeamAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;
    public MoonBeamAbility() : base(14)
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
    public override bool isUltimate()
    {
        return false;
    }
    public override void OnCast()
    {
        level = (int)Player.instance.abilityValues["ability.moonbeam.level"];
        maxCooldown = 14; //level < 4 ? level < 3 ? level < 2 ? 4 : 3 : 2 : 1;

        currentCooldown = maxCooldown;

        PlayerRabi rabi = (PlayerRabi)Player.instance;
        int beams = level < 5 ? 1 : 2;
        for (int i = 0; i < beams; i++)
        {
            MoonBeam moonBeam = PoolManager.Get<MoonBeam>();
            moonBeam.transform.position = new Vector3(rabi.transform.position.x, rabi.transform.position.y + 1.5f, 10f);
            moonBeam.transform.localEulerAngles = new Vector3(0, 0, 45f);
            Player.instance.despawneables.Add(moonBeam);

            Enemy e = Map.GetRandomClosebyEnemy();
            if (e == null) moonBeam.movDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            else moonBeam.movDir = (e.transform.position - rabi.transform.position);
        }
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