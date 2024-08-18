using System.Collections.Generic;

public abstract class StageTimeEvent
{
    public float time;

    public StageTimeEvent(float time)
    {
        this.time = time;
    }

    public abstract void Trigger();
}