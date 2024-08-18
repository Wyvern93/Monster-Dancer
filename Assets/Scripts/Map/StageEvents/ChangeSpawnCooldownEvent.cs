using System;
using System.Collections.Generic;

public class ChangeSpawnCooldownEvent : StageTimeEvent
{
    public int cooldown;
    public ChangeSpawnCooldownEvent(int cooldown,  float time) : base(time) 
    {
        this.cooldown = cooldown;
    }
    public override void Trigger()
    {
        Map.Instance.beatsBeforeWave = cooldown;
    }
}