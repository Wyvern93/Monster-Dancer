using System;
using UnityEngine;

[Serializable]
public abstract class Enhancement
{
    public abstract bool isUnique();
    public abstract float getRarity();
    public abstract int getPriority();
    public abstract void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus);

    public abstract string getId();

    public abstract string getName();

    public abstract int getLevel();

    public abstract string getType();

    public abstract Sprite getIcon();

    public virtual void OnUpdate() { }

    public abstract void OnEquip();

    public bool isUnlocked()
    {
        return SaveManager.PersistentSaveData.GetData<bool>($"enhancement.{getId()}.unlocked");
    }

    public abstract bool isAvailable();

    public abstract string GetDescription();
}