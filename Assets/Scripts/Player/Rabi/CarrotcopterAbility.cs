using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotcopterAbility : PlayerAbility, IPlayerProjectile
{
    CarrotCopter currentDrone;
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
        return Localization.GetLocalizedString("ability.rabi.carrotcopter.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotcopterAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "carrotcopter";
    }

    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.carrotcopter.level"];
        if (currentDrone == null)
        {
            CarrotCopter carrotcopter = PoolManager.Get<CarrotCopter>();
            carrotcopter.transform.position = Player.instance.transform.position;
            currentDrone = carrotcopter;
            Player.instance.despawneables.Add(carrotcopter);
        }
        else if (!currentDrone.gameObject.activeSelf)
        {
            CarrotCopter carrotcopter = PoolManager.Get<CarrotCopter>();
            carrotcopter.transform.position = Player.instance.transform.position;
            currentDrone = carrotcopter;
            Player.instance.despawneables.Add(carrotcopter);
        }

        maxCooldown = 4;
        currentCooldown = maxCooldown;
    }

    public override void OnEquip()
    {

    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}