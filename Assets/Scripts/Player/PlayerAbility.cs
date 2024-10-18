using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility
{
    public int maxCooldown = 0;
    public int currentCooldown = 0;
    public virtual bool isEvolved() { return false; }
    /*
    public PlayerAbility (int maxCD)
    {
        maxCooldown = maxCD;
        currentCooldown = 0;
    }*/
    public abstract bool isUltimate();
    public virtual Sprite GetIcon()
    {
        return IconList.instance.getAbilityIcon(getId());
    }
    public abstract void OnUpdate();
    public abstract void OnEquip();
    public abstract void OnCast();
    public abstract bool CanCast();
    public abstract string getAbilityName();
    public abstract string getAbilityDescription();
    public abstract string getId();

    public virtual void OnDespawn() { }

    public virtual Type getEvolutionItemType()
    {
        return null;
    }

    public virtual Type getEvolutionAbilityType() { return null; }

    public virtual Enhancement getEvolutionEnhancement()
    {
        return null;
    }

    public virtual int GetLevel()
    {
        return (int)Player.instance.abilityValues[$"ability.{getId()}.level"];
    }

    public virtual bool onPlayerPreHurt(float dmg) { return true; }
    public virtual void OnPlayerPostHurt() { }

    public abstract List<Enhancement> getEnhancementList();
}