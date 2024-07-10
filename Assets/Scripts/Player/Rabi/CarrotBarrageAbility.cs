using System.Collections.Generic;
using UnityEngine;

public class CarrotBarrageAbility : PlayerAbility
{
    
    public CarrotBarrageAbility(): base()
    {
        maxCooldown = 8;
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
        return Localization.GetLocalizedString("ability.rabi.carrotbarrage.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        throw new System.NotImplementedException();
    }

    public override string getID()
    {
        return "rabi.carrotbarrage";
    }

    public override void OnCast()
    {
        currentCooldown = maxCooldown;

        int numberOfCarrots = (int)Player.instance.abilityValues["carrot_barrage.projectiles"];

        for (int i = 0; i < numberOfCarrots; i++) 
        {
            Vector2 dir = Vector2.zero;
            if (numberOfCarrots == 4)
            {
                if (i == 0) CastCarrot(new Vector2(1,1));
                if (i == 1) CastCarrot(new Vector2(1, -1));
                if (i == 0) CastCarrot(new Vector2(-1, 1));
                if (i == 0) CastCarrot(new Vector2(-1, -1));
            }
            
        }
        
    }

    public void CastCarrot(Vector2 direction)
    {
        ExplosiveCarrot carrot = PoolManager.Get<ExplosiveCarrot>();
        carrot.transform.position = Player.instance.transform.position;
        carrot.Init(direction);
    }

    public override void OnEquip()
    {
        Player.instance.abilityValues.Add("carrot_barrage.projectiles", 4);
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}