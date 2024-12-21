using System.Collections.Generic;

public abstract class StageEvent
{
    public float time;

    public StageEvent(float time)
    {
        this.time = time;
    }

    public abstract void Trigger();

    public virtual List<EnemyType> GetEnemiesInWave()
    {
        return new List<EnemyType>();
    }
}