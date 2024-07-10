using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class WaveGroup
{
    public List<SpawnData> spawns;

    public void Spawn()
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            if (spawns[i].spawnType == SpawnType.AROUND_PLAYER)
            {
                Map.Instance.StartCoroutine(Map.SpawnEnemyAroundPlayer(spawns[i], i));
            }
            else
            {
                Map.Instance.StartCoroutine(Map.SpawnEnemyAtPos(spawns[i], i));
            }
        }
    }
}