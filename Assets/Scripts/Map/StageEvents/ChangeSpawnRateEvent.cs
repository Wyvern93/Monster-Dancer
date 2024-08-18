using System;
using System.Collections.Generic;

public class ChangeSpawnRateEvent : StageTimeEvent
{
    public int amount;
    public ChangeSpawnRateEvent(int amount,  float time) : base(time) 
    {
        this.amount = amount;
    }
    public override void Trigger()
    {
        Map.Instance.spawnRate = amount + (GameManager.runData.stageMulti * 2);
    }
}