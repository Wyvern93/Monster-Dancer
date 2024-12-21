using System;
using System.Collections.Generic;

public class RemoveEnemyEvent : StageEvent
{
    public EnemyType enemyType;
    public EnemyAIType ai;
    public RemoveEnemyEvent(EnemyType enemytype, EnemyAIType ai, float time) : base(time) 
    {
        enemyType = enemytype;
        this.ai = ai;
    }
    public override void Trigger()
    {
        Stage.Instance.spawnPool.RemoveAll(x=> x.enemyType == enemyType && x.AItype == ai);
    }
}