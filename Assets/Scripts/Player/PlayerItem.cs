using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerItem
{

    public PlayerItem()
    {
    }

    public virtual Sprite GetIcon()
    {
        return IconList.instance.getItemIcon(getId());
    }
    public abstract void OnUpdate();
    public abstract void OnEquip();
    public abstract void OnCast();
    public abstract bool CanCast();
    public abstract string getItemName();
    public abstract string getItemDescription();
    public abstract string getId();

    public virtual int GetLevel()
    {
        return (int)Player.instance.itemValues[$"item.{getId()}.level"];
    }

    public virtual float OnPreHeal(float amount)
    {
        return amount;
    }

    public virtual void OnHit(PlayerAbility source, float damage, Enemy target)
    { }

    public virtual void OnAttackHit(PlayerAttack source, float damage, Enemy target)
    { }

    public abstract List<Enhancement> getEnhancementList();
}