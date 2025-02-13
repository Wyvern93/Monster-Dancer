using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitEvent : StageEvent
{
    int beats;
    public WaitEvent(int beats)
    {
        this.beats = beats;
    }

    public override StageEventType getStageEventType()
    {
        return StageEventType.Wait;
    }

    public override IEnumerator Trigger(StageWave sourceWave)
    {
        int beatsPast = 0;
        while (true)
        {
            if (BeatManager.isBeat) beatsPast++;
            if (beatsPast >= beats) break;
            yield return null;
        }
        yield break;
    }
}