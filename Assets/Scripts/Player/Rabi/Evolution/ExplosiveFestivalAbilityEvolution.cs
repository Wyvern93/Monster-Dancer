using System.Collections.Generic;
using UnityEngine;

public class ExplosiveFestialAbilityEvolution : PlayerAbility
{
    private List<Firework> carrots = new List<Firework>();
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
        return Localization.GetLocalizedString("ability.rabi.explosivefestival.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new ExplosiveFestivalAbilityEnhancement() };
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "explosivefestival";
    }

    public override void OnCast()
    {
        carrots.Clear();
        baseCooldown = 6;
        currentCooldown = GetMaxCooldown();

        float dmg = 65;
        int numberOfCarrots = 5;

        float angleDiff = 360f / numberOfCarrots;
        for (int i = 0; i < numberOfCarrots; i++) 
        {
            Vector2 dir = BulletBase.angleToVector((angleDiff * i) + 45f);
            CastCarrot(dir, dmg, i == 0);
        }
        AudioController.PlaySound((Player.instance as PlayerRabi).fireworkSound);

    }

    public void CastCarrot(Vector2 direction, float damage, bool playSound)
    {
        Enemy enemy = Stage.GetRandomEnemy();
        if (enemy != null)
        {
            Firework carrot = PoolManager.Get<Firework>();
            carrot.abilitySource = this;
            carrot.transform.position = Player.instance.transform.position;
            Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());
            carrot.start = Player.instance.transform.position;
            carrot.target = enemy;
            carrot.end = enemy.transform.position;
        }
        else return;
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
        if (BeatManager.isBeat && currentCooldown > 0) currentCooldown--;
    }

    public override bool isEvolved()
    {
        return true;
    }
}