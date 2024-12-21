using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SpawnCircleHordeEvent : StageEvent
{
    public SpawnData data;
    public int number;
    public SpawnCircleHordeEvent(SpawnData data, int number, float time) : base(time)
    {
        this.data = data;
        this.number = number;
    }

    public override void Trigger()
    {
        float angle, x, y;
        angle = UnityEngine.Random.Range(0, 360f);
        x = Player.instance.transform.position.x + (14 * Mathf.Cos(angle));
        y = Player.instance.transform.position.y + (14 * Mathf.Sin(angle));

        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

        Vector3 playerPos = Player.instance.GetClosestPlayer(spawnPos) + (Vector3)UnityEngine.Random.insideUnitCircle * 2;
        Vector2 dir = playerPos - spawnPos;
        dir.Normalize();

        EnemyGroup group = PoolManager.Get<EnemyGroup>();
        group.aIType = EnemyAIType.CircleHorde;
        group.centerIsLead = true;
        group.transform.position = spawnPos;

        //Enemy enemy = Enemy.GetEnemyOfType(data.enemyType);
        //enemy.aiType = data.AItype;
        //enemy.SpawnIndex = 0;

        float size = 1.5f;
        for (int i = 0; i < number; i++)
        {
            Vector3 random = spawnPos + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.05f); //(Vector3)(UnityEngine.Random.insideUnitCircle * size) + spawnPos;
            Enemy e = Enemy.GetEnemyOfType(data.enemyType);
            e.aiType = EnemyAIType.CircleHorde;
            e.transform.position = random;
            e.eventMove = dir;
            e.group = group;
            group.enemies.Add(e);
            e.OnSpawn();
        }
        group.OnGroupInit();
    }

    public override List<EnemyType> GetEnemiesInWave()
    {
        List<EnemyType> enemies = new List<EnemyType>();
        for (int i = 0; i < number; i++)
        {
            enemies.Add(data.enemyType);
        }
        return enemies;
    }
}