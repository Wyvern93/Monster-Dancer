using System.Collections.Generic;
using UnityEngine;

public class CarrotJuiceAbility : PlayerAbility
{
    
    public CarrotJuiceAbility(): base(20)
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
        return Localization.GetLocalizedString("ability.rabi.carrotjuice.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotJuiceAbilityEnhancement() };
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.carrotJuice;
    }

    public override string getID()
    {
        return "rabi.carrotjuice";
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.carrotjuice.level"];
        maxCooldown = level < 2 ? 20 : 12; 
        currentCooldown = maxCooldown;

        CastBottle(new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)));
        if (level >= 4) CastBottle(new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)));
    }

    public void CastBottle(Vector2 direction)
    {
        CarrotJuiceBottle bottle = PoolManager.Get<CarrotJuiceBottle>();
        bottle.transform.position = Player.instance.transform.position;
        bottle.Init(direction);
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}