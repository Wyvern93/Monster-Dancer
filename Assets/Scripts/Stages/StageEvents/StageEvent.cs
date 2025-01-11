using System.Collections;
using System.Collections.Generic;

public abstract class StageEvent
{
    public EnemySpawnType enemySpawnType;
    public abstract StageEventType getStageEventType();

    public abstract IEnumerator Trigger(StageWave sourceWave);

}