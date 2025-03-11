using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PoolList", fileName = "PoolList")]
[Serializable]
public class PoolList : ScriptableObject
{
    public List<PoolData> pools;

    // Add method to allow adding PoolData from script
    public void AddPoolData(PoolData poolData)
    {
        pools.Add(poolData);
    }
}