using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility
{
    public int maxCooldown;
    public int currentCooldown;

    public PlayerAbility (int maxCD)
    {
        maxCooldown = maxCD;
        currentCooldown = 0;
    }

    public abstract bool isUltimate();
    public abstract Sprite GetIcon();
    public abstract void OnUpdate();
    public abstract void OnEquip();
    public abstract void OnCast();
    public abstract bool CanCast();
    public abstract string getAbilityName();
    public abstract string getAbilityDescription();
    public abstract string getID();

    public abstract List<Enhancement> getEnhancementList();
}