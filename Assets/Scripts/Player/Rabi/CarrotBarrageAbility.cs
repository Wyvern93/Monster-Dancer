using System.Collections.Generic;
using UnityEngine;

public class CarrotBarrageAbility : PlayerAbility
{
    private List<ExplosiveCarrot> carrots = new List<ExplosiveCarrot>();
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
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "carrotbarrage";
    }

    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.carrotbarrage.level"];
        carrots.Clear();
        maxCooldown = level < 6 ? level < 3 ? 10 : 8 : 6;
        currentCooldown = maxCooldown;

        float dmg = level < 4 ? level < 2 ? 25 : 40 : 65;
        int numberOfCarrots = level < 5 ? 3 : 5;

        float angleDiff = 360f / numberOfCarrots;
        for (int i = 0; i < numberOfCarrots; i++) 
        {
            Vector2 dir = BulletBase.angleToVector((angleDiff * i) + 45f);
            CastCarrot(dir, dmg, i == 0);
        }

    }

    public void CastCarrot(Vector2 direction, float damage, bool playSound)
    {
        ExplosiveCarrot carrot = PoolManager.Get<ExplosiveCarrot>();
        carrot.transform.position = Player.instance.transform.position;
        carrot.isSmall = false;
        carrot.Init(direction);
        carrot.dmg = damage;
        
        if (playSound) carrot.PlaySpin();
        Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());
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
        return typeof(FireworksKitItem);
    }

    public override Enhancement getEvolutionEnhancement()
    {
        return new ExplosiveFestivalAbilityEnhancement();
    }
}