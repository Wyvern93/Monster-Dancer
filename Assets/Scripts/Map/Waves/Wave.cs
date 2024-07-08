using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

[Serializable]
public class Wave
{
    [SerializeField] public List<WaveGroup> groups;
    public int getCost()
    {
        int cost = 0;
        foreach (WaveGroup group in groups)
        {
            foreach (SpawnData spawnData in group.spawns)
            {
                cost += Enemy.GetEnemyCost(spawnData.enemyType);
            }
        }
        return cost;
    }
    public void Spawn()
    {
        Debug.Log("Wave tries to spawn");
        Map.WaveNumberOfEnemies = GetEnemyCount();
        for (int i = 0; i < groups.Count; i++) 
        {
            groups[i].Spawn();
        }
        
    }

    public int GetEnemyCount()
    {
        int i = 0;
        foreach (WaveGroup group in groups)
        {
            i += group.spawns.Count;
        }
        return i;
    }
}