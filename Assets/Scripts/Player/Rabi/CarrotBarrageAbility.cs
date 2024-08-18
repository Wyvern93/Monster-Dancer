using System.Collections.Generic;
using UnityEngine;

public class CarrotBarrageAbility : PlayerAbility
{
    
    public CarrotBarrageAbility(): base(8)
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
        return Localization.GetLocalizedString("ability.rabi.carrotbarrage.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new CarrotBarrageAbilityEnhancement() };
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.carrotBarrage;
    }

    public override string getID()
    {
        return "rabi.carrotbarrage";
    }

    public override void OnCast()
    {
        currentCooldown = maxCooldown;

        int level = (int)Player.instance.abilityValues["ability.carrotbarrage.level"];
        int numberOfCarrots = 4 + ((level - 1) * 2);

        for (int i = 0; i < numberOfCarrots; i++) 
        {
            Vector2 dir = Vector2.zero;
            if (level == 1)
            {
                if (i == 0) CastCarrot(new Vector2(1,1));
                if (i == 1) CastCarrot(new Vector2(1, -1));
                if (i == 2) CastCarrot(new Vector2(-1, 1));
                if (i == 3) CastCarrot(new Vector2(-1, -1));
            }

            if (level == 2)
            {
                if (i == 0) CastCarrot(new Vector2(1, 1));
                if (i == 1) CastCarrot(new Vector2(1, -1));
                if (i == 2) CastCarrot(new Vector2(-1, 1));
                if (i == 3) CastCarrot(new Vector2(-1, -1));
                if (i == 4) CastCarrot(new Vector2(0, 1));
                if (i == 5) CastCarrot(new Vector2(0, -1));
            }

            if (level == 3)
            {
                if (i == 0) CastCarrot(new Vector2(1, 1));
                if (i == 1) CastCarrot(new Vector2(1, -1));
                if (i == 2) CastCarrot(new Vector2(-1, 1));
                if (i == 3) CastCarrot(new Vector2(-1, -1));
                if (i == 4) CastCarrot(new Vector2(0, 1));
                if (i == 5) CastCarrot(new Vector2(0, -1));
                if (i == 6) CastCarrot(new Vector2(1, 0));
                if (i == 7) CastCarrot(new Vector2(-1, 0));
            }

            if (level == 4)
            {
                if (i == 0) CastCarrot(new Vector2(1, 1));
                if (i == 1) CastCarrot(new Vector2(1, -1));
                if (i == 2) CastCarrot(new Vector2(-1, 1));
                if (i == 3) CastCarrot(new Vector2(-1, -1));
                if (i == 4) CastCarrot(new Vector2(0, 1));
                if (i == 5) CastCarrot(new Vector2(0, -1));
                if (i == 6) CastCarrot(new Vector2(1, 0));
                if (i == 7) CastCarrot(new Vector2(-1, 0));

                if (i == 8) CastCarrot(new Vector2(2, 2));
                if (i == 9) CastCarrot(new Vector2(2, -2));
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
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}