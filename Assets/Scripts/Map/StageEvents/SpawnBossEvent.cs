using System;
using System.Collections.Generic;

public class SpawnBossEvent : StageTimeEvent
{
    public EnemyType enemyType;
    public SpawnBossEvent(EnemyType enemytype, float time) : base(time) 
    {
        enemyType = enemytype;
    }
    public override void Trigger()
    {
        SpawnData spawnData = new SpawnData();
        spawnData.spawnType = SpawnType.AROUND_PLAYER;
        spawnData.enemyType = enemyType;
        
        Map.Instance.SpawnBoss(spawnData);
    }
}