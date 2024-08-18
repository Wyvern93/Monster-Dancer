using System;
using System.Collections.Generic;

public class RemoveEnemyEvent : StageTimeEvent
{
    public EnemyType enemyType;
    public int ai;
    public RemoveEnemyEvent(EnemyType enemytype, int ai, float time) : base(time) 
    {
        enemyType = enemytype;
        this.ai = ai;
    }
    public override void Trigger()
    {
        Map.Instance.spawnPool.RemoveAll(x=> x.enemyType == enemyType && x.AItype == ai);
    }
}