using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : PlayerInventoryObject
{
    // Base values
    public float baseCooldown;
    public float baseAttackSpeed;
    public float baseAmmo;
    public float baseDamage;
    public float baseReach;
    public float baseSpeed;
    public float baseDuration;
    public float baseSize; // For AOE too
    public float baseKnockback;
    public float baseCritChance;
    public float baseSpread;

    public int level;
    public float currentExp, maxExp;

    public float currentCooldown = 0;
    public float currentAttackSpeedCD = 0;
    public int currentAmmo = 0;
    public virtual bool isEvolved() { return false; }

    public PlayerItem[] equippedItems;
    public Dictionary<string, float> itemValues;

    public virtual float GetMaxCooldown()
    {
        float rawCooldown = Mathf.Clamp((baseCooldown + itemValues["bonusCooldown"]) * itemValues["cooldownMultiplier"], 0, 1000);
        rawCooldown = Mathf.Round(rawCooldown / 0.25f) * 0.25f;
        rawCooldown = Mathf.Clamp(rawCooldown, 0.25f, 1000);
        return rawCooldown;
    }

    public virtual BeatManager.BeatType getBeatTrigger()
    {
        return BeatManager.BeatType.Beat;
    }

    public float GetBeatSize()
    {
        BeatManager.BeatType beatTrigger = getBeatTrigger();
        switch (beatTrigger)
        {
            default:
            case BeatManager.BeatType.Beat: return 1f;
            case BeatManager.BeatType.Mid: return 0.5f;
            case BeatManager.BeatType.Quarter: return 0.25f;
            case BeatManager.BeatType.MidCompass: return 2f;
            case BeatManager.BeatType.Compass: return 4f;
        }
    }

    public virtual float GetAttackSpeed()
    {
        float rawAttackSpeed = Mathf.Clamp((baseAttackSpeed + itemValues["bonusAtkSpeed"]) * itemValues["atkSpeedMultiplier"], 0, 100);
        rawAttackSpeed = Mathf.Round(rawAttackSpeed / 0.25f) * 0.25f;
        rawAttackSpeed = Mathf.Clamp(rawAttackSpeed, 0.25f, 100);
        return rawAttackSpeed;
    }

    public virtual int GetMaxAmmo()
    {
        return (int)Mathf.Clamp(baseAmmo + itemValues["ammoExtra"], 1, 8);
    }

    public virtual float GetSpread()
    {
        float rawSpread = Mathf.Clamp(baseSpread * itemValues["spreadMultiplier"], 0, 360);
        return rawSpread;
    }

    public virtual float GetCritChance()
    {
        return Mathf.Clamp(baseCritChance + itemValues["critChance"], 0, 100f);
    }

    public virtual float GetDamage()
    {
        return Mathf.Clamp(baseDamage * itemValues["damageMultiplier"], 1, 10000);
    }

    public virtual float GetSpeed()
    {
        return Mathf.Clamp(baseSpeed * itemValues["speedMultiplier"], 1, 1000);
    }

    public virtual float GetDuration()
    {
        float rawDuration = Mathf.Clamp((baseDuration + itemValues["bonusDuration"]) * itemValues["durationMultiplier"], 0.25f, 100);
        return Mathf.Round(rawDuration / 0.25f) * 0.25f;
    }

    public virtual float GetKnockback()
    {
        return Mathf.Clamp(baseKnockback * itemValues["knockbackMultiplier"], 0, 100);
    }

    public virtual float GetReach()
    {
        return Mathf.Clamp(baseReach * itemValues["reachMultiplier"], 0, 12);
    }

    public virtual float GetSize()
    {
        return Mathf.Clamp(baseSize * itemValues["sizeMultiplier"], 0, 100);
    }

    public virtual Color GetTooltipColor()
    {
        return Color.white;
    }

    public bool hasItem(Type itemType)
    {
        foreach (PlayerItem i in equippedItems)
        {
            if (i == null) continue;
            if (i.GetType() == itemType) return true;
        }
        return false;
    }

    public int CalculateExpCurve(int lv)
    {
        if (lv == 1) return 50;

        //float a = Mathf.Pow(4 * (lv + 1), 1.7f);
        //float b = Mathf.Pow(4 * lv + 1, 1.7f);
        //int exp = (int)(Mathf.Round(a) - Mathf.Round(b));
        int value = (int)(maxExp + (10 * lv));
        
        return value;
    }

    public int TotalExpForLevelRange(int startLevel, int endLevel)
    {
        int totalExp = 0;
        for (int lv = startLevel; lv < endLevel; lv++)
        {
            totalExp += CalculateExpCurve(lv);
        }
        return totalExp;
    }

    public void AddExp(int n)
    {
        //n = (int)(n * instance.currentStats.ExpMulti);
        //if (Player.instance.isDead) return;
        currentExp += n;
        if (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            level++;
            OnLevelUp();
        }
        UpdateExp();
    }

    public void OnLevelUp()
    {
        maxExp = CalculateExpCurve(level);
        AudioController.PlaySound(AudioController.instance.sounds.playerLvlUpSfx);
        Player.instance.StartCoroutine(OpenEnhancementMenuCoroutine());
    }

    protected IEnumerator OpenEnhancementMenuCoroutine()
    {
        while (!BeatManager.isBeat) yield return null;
        BeatManager.isBeat = false;
        EnhancementMenu.instance.Open();
    }

    public void UpdateExp()
    {
        float exp = currentExp / maxExp;
        UIManager.Instance.PlayerUI.UpdateSkillExp(this, level, exp);
    }

    public PlayerAbility()
    {
        level = 1;
        maxExp = CalculateExpCurve(level);
        currentExp = 0;
        equippedItems = new PlayerItem[6];
        itemValues = new Dictionary<string, float>
        {
            { "orbitalSpeed", 1.0f },
            { "orbitalDamage", 1.0f },
            { "explosionDamage", 1.0f },
            { "explosionSize", 1.0f },
            { "burnDamage", 1.0f },
            { "burnDuration", 3f },
            { "cooldownMultiplier", 1.0f },
            { "bonusCooldown", 0.0f },
            { "atkSpeedMultiplier", 1.0f },
            { "bonusAtkSpeed", 0.0f },
            { "ammoExtra", 0.0f },
            { "damageMultiplier", 1.0f },
            { "reachMultiplier", 1.0f },
            { "speedMultiplier", 1.0f },
            { "durationMultiplier", 1.0f },
            { "bonusDuration", 0.0f },
            { "sizeMultiplier", 1.0f },
            { "knockbackMultiplier", 1.0f },
            { "critChance", 0.0f },
            { "spreadMultiplier", 1.0f },
            { "slowMultiplier", 1.0f }
        };
    }

    public abstract bool isUltimate();
    public override Sprite GetIcon()
    {
        return IconList.instance.getAbilityIcon(getId());
    }
    public abstract void OnUpdate();
    public virtual void OnEquip() { }
    public abstract void OnCast();
    public abstract bool CanCast();
    public abstract string getAbilityName();
    public abstract string getAbilityDescription();

    public virtual string getTags() { return ""; }

    public virtual void OnSelect()
    {
        UIManager.Instance.PlayerUI.SetAmmoIcons(GetReloadIcon());
    }
    public virtual void OnChange() 
    {
        if (currentAmmo < GetMaxAmmo() && currentCooldown == 0)
        {
            currentAmmo = GetMaxAmmo();
            currentCooldown = GetMaxCooldown();
            currentAttackSpeedCD = GetAttackSpeed();
        }
    }

    public virtual void OnDespawn() { }

    public bool canEvolve()
    {
        StarGemType star = getEvolutionStarType();
        switch (star)
        {
            default: return false;
            case StarGemType.Fire: return Player.instance.fireStars > 0;
            case StarGemType.Water: return Player.instance.waterStars > 0;
            case StarGemType.Earth: return Player.instance.earthStars > 0;
            case StarGemType.Wind: return Player.instance.windStars > 0;
            case StarGemType.Light: return Player.instance.lightStars > 0;
            case StarGemType.Darkness: return Player.instance.darkStars > 0;
        }
    }

    public virtual StarGemType getEvolutionStarType()
    {
        return StarGemType.Fire;
    }

    public string AddStat(string statName, float baseValue, float calculatedValue, bool highBetter, string unit = "")
    {
        float parsedBaseValue = MathF.Round(baseValue, 2);
        float parsedCalculatedValue = MathF.Round(calculatedValue, 2);

        if (parsedBaseValue == parsedCalculatedValue)
        {
            return $"<color=#888888>{statName}: <color=\"white\">{parsedBaseValue}{unit}</color>\n";
        }

        bool isHigher = calculatedValue > baseValue;
        string color = (highBetter ? isHigher : !isHigher) ? "<color=#00FF00>" : "<color=#FF0000>";

        return $"<color=#888888>{statName}: <color=\"white\">{parsedBaseValue} -> {color}{parsedCalculatedValue}</color>{unit}</color>\n";
    }

    public bool IsCurrentWeaponSelected()
    {
        if (Player.instance.equippedPassiveAbilities[Player.instance.currentWeapon].GetType() == this.GetType()) return true;
        else return false;
    }

    public virtual Sprite GetReloadIcon()
    {
        return IconList.instance.getReloadIcon(getId());
    }

    protected string GetStarColorText()
    {
        switch (getEvolutionStarType())
        {
            default:
            case StarGemType.Fire: return "<color=#FF0000>";
            case StarGemType.Water: return "<color=#0000FF>";
            case StarGemType.Earth: return "<color=#444400>";
            case StarGemType.Wind: return "<color=#00FF00>";
            case StarGemType.Light: return "<color=#FFFF00>";
            case StarGemType.Darkness: return "<color=#4400FF>";
        }
    }

    public virtual Color GetRechargeColor()
    {
        return Color.white;
    }

    public virtual Type getEvolutionAbilityType() { return null; }

    public virtual Enhancement getEvolutionEnhancement()
    {
        return null;
    }

    public virtual bool onPlayerPreHurt(float dmg) { return true; }
    public virtual void OnPlayerPostHurt() { }

    public abstract List<Enhancement> getEnhancementList();
}