using System;
using System.Collections.Generic;

public class SpawnUniqueEnemyEvent : StageEvent
{
    public EnemyType enemyType;
    public SpawnUniqueEnemyEvent(EnemyType enemytype, float time) : base(time) 
    {
        enemyType = enemytype;
    }
    public override void Trigger()
    {
        SpawnData spawnData = new SpawnData();
        spawnData.spawnType = SpawnType.AROUND_PLAYER;
        spawnData.enemyType = enemyType;
        
        Stage.SpawnUniqueEnemy(spawnData);
    }
}