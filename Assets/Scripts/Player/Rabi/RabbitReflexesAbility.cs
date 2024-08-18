using System.Collections.Generic;
using UnityEngine;

public class RabbitReflexesAbility : PlayerAbility
{
    public int minCooldown = 1;
    int level;

    public int charges;
    public RabbitReflexesAbility() : base(0)
    {
    }

    public override bool CanCast()
    {
        return true;
    }

    public override string getAbilityDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string getAbilityName()
    {
        return Localization.GetLocalizedString("ability.rabi.rabbitreflexes.name");
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.rabbitReflexes;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new RabbitReflexesAbilityEnhancement() };
    }

    public override string getID()
    {
        return "rabi.rabbitreflexes";
    }

    public override void OnCast()
    {
        Player.instance.CalculateStats();
        level = (int)Player.instance.abilityValues["ability.rabbitreflexes.level"];
        Player.instance.currentStats.Atk += ((float)charges * (level < 2 ? 0.02f : 0.05f));
    }

    public void OnDamage()
    {
        charges = charges / 2;
    }

    public void OnBulletDespawn(Bullet bullet)
    {
        level = (int)Player.instance.abilityValues["ability.rabbitreflexes.level"];
        if (charges < 20) charges += 1;
        if (level >= 4) bullet.lifetime /= 2;
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;

    }
}