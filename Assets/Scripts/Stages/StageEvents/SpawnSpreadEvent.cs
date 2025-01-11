using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnSpreadEvent : StageEvent
{
    public int number;
    public SpawnSpreadEvent(int number, EnemySpawnType spawnType)
    {
        this.number = number;
        enemySpawnType = spawnType;
    }

    public override IEnumerator Trigger(StageWave sourceWave)
    {
        StartSpreadEvent(sourceWave);
        yield break;
    }

    private void StartSpreadEvent(StageWave sourceWave)
    {
        Stage.Instance.StartCoroutine(SpreadCoroutine(sourceWave));
    }

    private IEnumerator SpreadCoroutine(StageWave sourceWave)
    {
        EnemyGroup group = PoolManager.Get<EnemyGroup>();
        group.aIType = EnemyAIType.Spread;
        group.centerIsLead = true;

        EnemyType enemyType = sourceWave.getEnemyFromSpawnType(enemySpawnType);

        int n = 0;
        EnemyClass enemyClass = Enemy.enemyClassFromSpawnType(enemySpawnType);
        int wait = (enemyClass == EnemyClass.Runner ? 3 : enemyClass == EnemyClass.Bomber ? 8 : 6);
        int spread = (int)(number / 4f);
        if (spread < 1) spread = 1;
        for (int i = 0; i < number; i++)
        {
            n++;
            Stage.SpawnSpreadEnemy(enemyType, group);
            if (n >= spread)
            {
                n = 0;
                yield return new WaitForSeconds(BeatManager.GetBeatDuration() * wait);
            }
        }

        group.OnGroupInit();
        yield break;
    }

    public override StageEventType getStageEventType()
    {
        return StageEventType.SpawnSpreadGroup;
    }
}