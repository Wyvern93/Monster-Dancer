using System;
using System.Collections;
using System.Collections.Generic;

public class SpawnBossEvent : StageEvent
{
    public SpawnBossEvent()
    {
    }

    public override StageEventType getStageEventType()
    {
        return StageEventType.SpawnBoss;
    }

    public override IEnumerator Trigger(StageWave sourceWave)
    {
        SpawnData spawnData = new SpawnData();
        spawnData.spawnType = SpawnType.AROUND_PLAYER;
        spawnData.enemyType = sourceWave.waveData.specialSpawnEnemy;
        
        Stage.Instance.SpawnBoss(spawnData);
        yield break;
    }
}