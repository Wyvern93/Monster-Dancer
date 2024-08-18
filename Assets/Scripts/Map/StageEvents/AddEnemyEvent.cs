using System;
using System.Collections.Generic;

public class AddEnemyEvent : StageTimeEvent
{
    public EnemyType enemyType;
    public float weight;
    public int ai;
    public AddEnemyEvent(EnemyType enemytype, float weight, int ai, float time) : base(time) 
    {
        enemyType = enemytype;
        this.weight = weight;
        this.ai = ai;
    }
    public override void Trigger()
    {
        SpawnData spawnData = new SpawnData();
        spawnData.spawnType = SpawnType.AROUND_PLAYER;
        spawnData.enemyType = enemyType;
        spawnData.AItype = ai;
        spawnData.weight = weight;
        
        Map.Instance.spawnPool.Add(spawnData);
    }
}