using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerItem : PlayerInventoryObject
{
    public PlayerItem()
    {
    }

    public override Sprite GetIcon()
    {
        return IconList.instance.getItemIcon(getId());
    }
    public abstract void OnUpdate();
    public virtual void OnEquip(PlayerAbility ability, int slot)
    {
        
    }
    public virtual void OnUnequip(PlayerAbility ability, int slot)
    {
        
    }
    public abstract void OnCast();
    public abstract bool CanCast();
    public abstract string getItemName();
    public abstract string getItemDescription();

    public virtual Color GetTooltipColor()
    {
        return WeightToRarity(getRarity());
    }

    public virtual int getRarity()
    {
        return 4;
    }

    private Color WeightToRarity(int rarity)
    {
        switch (rarity)
        {
            case 1:
                return Color.yellow;
            case 2:
                return Color.blue;
            case 3:
                return Color.green;
            default:
            case 4:
                return Color.white;
        }
    }

    public virtual string getTags() { return ""; }

    public string AddStat(string statName, float value, bool highBetter, string unit = "")
    {
        float parsedValue = MathF.Round(value, 2);

        string color = "";
        if (value > 0 && highBetter) color = "<color=#00FF00>";
        if (value < 0 && !highBetter) color = "<color=#00FF00>";

        if (value > 0 && !highBetter) color = "<color=#FF0000>";
        if (value < 0 && highBetter) color = "<color=#FF0000>";
        string valueEnd = value > 0 ? $"+{color}{parsedValue}" : $"{color}{parsedValue}";

        return $"<color=#888888>{statName}: {valueEnd}</color>{unit}</color>\n";
    }

    public virtual float OnPreHeal(float amount)
    {
        return amount;
    }

    public virtual void OnHit(PlayerAbility source, float damage, Enemy target, bool critical)
    { }

    public abstract List<Enhancement> getEnhancementList();
}