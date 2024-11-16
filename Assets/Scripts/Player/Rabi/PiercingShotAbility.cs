using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingShotAbility : PlayerAbility, IPlayerProjectile
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
        return Localization.GetLocalizedString("ability.rabi.piercingshot.name"); // Unused
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new PiercingShotAbilityEnhancement() }; // Used for the menu
    }
    public override bool isUltimate()
    {
        return false;
    }
    public override string getId()
    {
        return "piercingshot";
    }

    public override void OnCast()
    {
        int level = (int)Player.instance.abilityValues["ability.piercingshot.level"];
        maxCooldown = level < 3 ? 10 : 7;
        Player.instance.StartCoroutine(CastCoroutine(level));
    }

    public IEnumerator CastCoroutine(int level)
    {
        float time = BeatManager.GetBeatDuration() * 2f;
        AudioController.PlaySound((Player.instance as PlayerRabi).piercingShotChargeSound);

        while (time > 0)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;

        ShootCarrot(difference, level);
        yield break;
    }

    public void ShootCarrot(Vector2 direction, int level)
    {
        PiercingShot carrot = PoolManager.Get<PiercingShot>();
        carrot.transform.position = Player.instance.transform.position;
        carrot.dir = direction;
        carrot.transform.position = Player.instance.transform.position;
        carrot.isSmall = level < 5;
        carrot.PlayAnimation();
        AudioController.PlaySound(carrot.piercingShotSound);

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

    public override Enhancement getEvolutionEnhancement()
    {
        return new FlamingDrillAbilityEnhancement();
    }
}