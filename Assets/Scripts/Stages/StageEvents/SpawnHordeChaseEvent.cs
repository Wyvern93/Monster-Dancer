using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnHordeChaseEvent : StageEvent
{
    public int number;
    public SpawnHordeChaseEvent(int number, EnemySpawnType spawnType)
    {
        this.number = number;
        enemySpawnType = spawnType;
    }

    public override IEnumerator Trigger(StageWave sourceWave)
    {
        EnemyGroup group = PoolManager.Get<EnemyGroup>();
        group.aIType = EnemyAIType.HordeChase;
        group.centerIsLead = true;
        group.enemies.Clear();

        EnemyType enemyType = sourceWave.getEnemyFromSpawnType(enemySpawnType);

        Stage.SpawnCircleHorde(enemyType, group, number, true);

        group.OnGroupInit();
        yield break;
    }

    /*
    public List<EnemyType> GetEnemiesInWave(StageWave sourceWave)
    {
        List<EnemyType> enemies = new List<EnemyType>();
        for (int i = 0; i < number; i++)
        {
            enemies.Add(data.enemyType);
        }
        return enemies;
    }
    */
    public override StageEventType getStageEventType()
    {
        return StageEventType.SpawnHordeChaseEvent;
    }
}