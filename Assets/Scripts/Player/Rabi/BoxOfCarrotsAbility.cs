using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOfCarrotsAbility : PlayerAbility
{

    public BoxOfCarrotsAbility() : base(20)
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
        return Localization.GetLocalizedString("ability.rabi.boxofcarrots.name");
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new BoxOfCarrotsAbilityEnhancement() };
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.boxofCarrots;
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getID()
    {
        return "rabi.boxofcarrots";
    }

    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.boxofcarrots.level"];
        maxCooldown = level < 3 ? 18 : 12;
        currentCooldown = maxCooldown;

        int numboxes = level < 7 ? 1 : 2;
        for (int i = 0; i < numboxes; i++)
        {
            BoxOfCarrots boxOfCarrots = PoolManager.Get<BoxOfCarrots>();
            Vector2 position = Player.instance.transform.position + (Vector3)(Random.insideUnitCircle * 5);
            boxOfCarrots.transform.position = position;
            Player.instance.despawneables.Add(boxOfCarrots.GetComponent<IDespawneable>());
        }
    }

    public override void OnEquip()
    {

    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }
}