using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Data/PoolData", fileName = "PoolData")]
public class PoolData : ScriptableObject
{
    public GameObject prefab;
    public Component component;
    public int count;

    // Method to get the component from the prefab (to be used at runtime)
    public Component GetComponentFromPrefab()
    {
        if (prefab != null)
        {
            // Use GetComponent on the prefab to fetch the correct component
            return prefab.GetComponent(component.GetType());
        }
        return null;
    }

    public void CreatePool()
    {
        PoolManager.CreatePool(component.GetType(), prefab, count);
    }

    public void DestroyPool()
    {
        PoolManager.RemovePool(component.GetType());
    }
}