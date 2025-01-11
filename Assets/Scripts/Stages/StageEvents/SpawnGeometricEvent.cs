using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SpawnGeometricEvent : StageEvent
{
    public int number;
    public SpawnGeometricEvent(int number, EnemySpawnType spawnType)
    {
        this.number = number;
        enemySpawnType = spawnType;
    }

    public override IEnumerator Trigger(StageWave sourceWave)
    {
        Vector3 playerPos = Player.instance.transform.position;
        Vector3 spawnPos = PlayerCamera.instance.transform.position; //playerPos + (Vector3)(UnityEngine.Random.insideUnitCircle.normalized * 6);
        spawnPos.z = 0;

        Vector2 dir = playerPos - spawnPos;
        dir.Normalize();

        EnemyType enemyType = sourceWave.getEnemyFromSpawnType(enemySpawnType);

        EnemyGroup enemyGroup = PoolManager.Get<EnemyGroup>();
        enemyGroup.aIType = EnemyAIType.Orbital;
        enemyGroup.centerIsLead = true;
        enemyGroup.transform.position = spawnPos;

        ArchetypeStats stats = new ArchetypeStats(Enemy.enemyData[enemyType].archetype).getStatsAtWave(Stage.Instance.currentWave);
        enemyGroup.orbitSpeed = stats.baseSpeed;

        //Enemy enemy = Enemy.GetEnemyOfType(data.enemyType);
        //enemy.aiType = data.AItype;
        //enemy.SpawnIndex = 0;

        

        yield return Stage.SpawnEnemiesGeometric(enemyType, enemyGroup, number);
        
        //for (int i = 0; i < number; i++)
        //{
        //    Vector3 random = spawnPos + ((Vector3)(BulletBase.angleToVector((360f / number) * i))* 4f); //(Vector3)(UnityEngine.Random.insideUnitCircle * size) + spawnPos;
        //    Stage.SpawnUniqueEnemy(new SpawnData() { AItype = EnemyAIType.HordeChase, enemyType = enemyType, spawnPosition = random }, enemyGroup);
        //}
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
        return StageEventType.SpawnGeometricEvent;
    }
}