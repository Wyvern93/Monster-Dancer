using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamingDrillAbilityEvolution : PlayerAbility, IPlayerProjectile
{
    public override bool CanCast()
    {
        return currentCooldown == 0;
    }

    public override string getAbilityDescription()
    {
        throw new System.NotImplementedException(); // This is unused since it's used the AbilityEnhancement
    }

    public override string getAbilityName()
    {
        return Localization.GetLocalizedString("ability.rabi.flamingdrill.name"); // Unused
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new FlamingDrillAbilityEnhancement() }; // Used for the menu
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "flamingdrill";
    }

    public override void OnCast()
    {
        maxCooldown = 7;
        currentCooldown = maxCooldown;
        Player.instance.StartCoroutine(CastCoroutine());
    }

    public IEnumerator CastCoroutine()
    {
        float time = BeatManager.GetBeatDuration() * 2f;
        AudioController.PlaySound((Player.instance as PlayerRabi).flamingDrillChargeSound);

        while (time > 0)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;

        ShootCarrot(difference);
        yield break;
    }

    public void ShootCarrot(Vector2 direction)
    {
        FlamingDrill carrot = PoolManager.Get<FlamingDrill>();
        carrot.transform.position = Player.instance.transform.position;
        carrot.dir = direction;
        carrot.transform.position = Player.instance.transform.position;
        carrot.PlayAnimation();
        AudioController.PlaySound(carrot.flamingDrillSound);

        Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());
    }

    public override void OnEquip() { } // Unusued

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat && currentCooldown > 0) currentCooldown--;
    }

    public override System.Type getEvolutionItemType()
    {
        return typeof(HotSauceBottleItem);
    }

    public override bool isEvolved()
    {
        return true;
    }
}