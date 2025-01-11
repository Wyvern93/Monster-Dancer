using System;
using System.Collections.Generic;
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
                Stage.Instance.StartCoroutine(Stage.SpawnEnemyAroundPlayer(spawns[i], i));
            }
            else
            {
                Stage.Instance.StartCoroutine(Stage.SpawnEnemyAtPos(spawns[i], i));
            }
        }
    }
}