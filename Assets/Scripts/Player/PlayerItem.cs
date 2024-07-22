using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerItem
{

    public PlayerItem()
    {
    }

    public abstract Sprite GetIcon();
    public abstract void OnUpdate();
    public abstract void OnEquip();
    public abstract void OnCast();
    public abstract bool CanCast();
    public abstract string getItemName();
    public abstract string getItemDescription();
    public abstract string getID();

    public abstract List<Enhancement> getEnhancementList();
}