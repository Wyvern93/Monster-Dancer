using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SpawnCircleHordeEvent : StageEvent
{
    public int number;
    public SpawnCircleHordeEvent(int number, EnemySpawnType spawnType)
    {
        this.number = number;
        enemySpawnType = spawnType;
    }

    public override IEnumerator Trigger(StageWave sourceWave)
    {
        EnemyGroup group = PoolManager.Get<EnemyGroup>();
        group.aIType = EnemyAIType.CircleHorde;
        group.centerIsLead = true;
        group.enemies.Clear();

        EnemyType enemyType = sourceWave.getEnemyFromSpawnType(enemySpawnType);

        Stage.SpawnCircleHorde(enemyType, group, number, false);
        
        group.OnGroupInit();
        yield break;
    }

    public override StageEventType getStageEventType()
    {
        return StageEventType.SpawnCircleHordeGroup;
    }
}