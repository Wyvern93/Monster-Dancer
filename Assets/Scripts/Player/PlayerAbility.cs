using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility
{
    public float maxCooldown = 0;
    public float currentCooldown = 0;

    public float maxAttackSpeedCD = 0;
    public float currentAttackSpeedCD = 0;

    public int currentAmmo = 0;
    public int maxAmmo = 0;
    public virtual bool isEvolved() { return false; }

    public abstract bool isUltimate();
    public virtual Sprite GetIcon()
    {
        return IconList.instance.getAbilityIcon(getId());
    }
    public abstract void OnUpdate();
    public virtual void OnEquip() { }
    public abstract void OnCast();
    public abstract bool CanCast();
    public abstract string getAbilityName();
    public abstract string getAbilityDescription();
    public abstract string getId();

    public virtual void OnSelect()
    {
        UIManager.Instance.PlayerUI.SetAmmoIcons(GetReloadIcon());
    }
    public virtual void OnChange() 
    {
    }

    public virtual void OnDespawn() { }

    public virtual Type getEvolutionItemType()
    {
        return null;
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

    public virtual Color GetRechargeColor()
    {
        return Color.white;
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