using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class WavePreset
{
    public List<StageEvent> events;
    private static List<WavePreset> stage1aPresets;

    public IEnumerator PlayWave(StageWave sourceWave)
    {
        foreach (StageEvent stage in events)
        {
            yield return stage.Trigger(sourceWave);
        }
    }
}