using System;
using UnityEngine;

[Serializable]
public abstract class Enhancement
{
    public bool unique;
    public abstract void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus);

    public abstract string getId();

    public virtual void OnUpdate() { }

    public abstract void OnEquip();

    public bool isUnlocked()
    {
        return SaveManager.PersistentSaveData.GetData<bool>($"enhancement.{getId()}.unlocked");
    }
}