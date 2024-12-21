using System;
using System.Collections.Generic;

public class AddEnemyEvent : StageEvent
{
    public EnemyType enemyType;
    public float weight;
    public EnemyAIType ai;
    public AddEnemyEvent(EnemyType enemytype, float weight, EnemyAIType ai, float time) : base(time) 
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
        
        Stage.Instance.spawnPool.Add(spawnData);
    }
}