using System.Collections.Generic;
using UnityEngine;

public class CarrotJuiceAbility : PlayerAbility
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
        return Localization.GetLocalizedString("ability.rabi.carrotjuice.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotJuiceAbilityEnhancement() };
    }

    public override string getId()
    {
        return "carrotjuice";
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.carrotjuice.level"];
        maxCooldown = level < 3 ? 16 : 12; 
        currentCooldown = maxCooldown;

        CastBottle(Random.insideUnitCircle * 6f);
        if (level >= 7) CastBottle(Random.insideUnitCircle * 6f);
    }

    public void CastBottle(Vector2 direction)
    {
        CarrotJuiceBottle bottle = PoolManager.Get<CarrotJuiceBottle>();
        bottle.transform.position = Player.instance.transform.position;
        bottle.Init(direction);
        Player.instance.despawneables.Add(bottle);
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }

    public override System.Type getEvolutionItemType()
    {
        return typeof(BlessedFigureItem);
    }
}