using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEliteEvent : StageEvent
{
    public SpawnEliteEvent()
    {
    }

    public override StageEventType getStageEventType()
    {
        return StageEventType.SpawnElite;
    }

    public override IEnumerator Trigger(StageWave sourceWave)
    {
        Debug.Log("trigger elite");
        SpawnData spawnData = new SpawnData();
        spawnData.spawnType = SpawnType.AROUND_PLAYER;
        spawnData.enemyType = sourceWave.waveData.specialSpawnEnemy;

        Stage.SpawnElite(spawnData);
        yield return new WaitForSeconds(2f);
        yield break;
    }
}