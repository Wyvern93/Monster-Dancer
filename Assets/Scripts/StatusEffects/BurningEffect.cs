using System.Collections.Generic;
using UnityEngine;

public class BurningEffect
{
    List<BurnSource> sources;
    public int duration;

    public BurningEffect()
    {
        sources = new List<BurnSource>();
    }

    public void Clear()
    {
        sources.Clear();
    }

    public void AddBurn(BurnSource source)
    {
        sources.Add(source);
    }

    public bool isBurning()
    {
        if (sources.Count > 0) return true;
        else return false;
    }

    public float OnBurnTick()
    {
        float highestDamage = 0;
        List<BurnSource> toRemove = new List<BurnSource> ();
        foreach (BurnSource burnSource in sources)
        {
            if (burnSource.duration <= 0)
            {
                toRemove.Add(burnSource);
                continue;
            }

            burnSource.duration -= 0.25f;
            if (burnSource.damage > highestDamage) highestDamage = burnSource.damage;
        }
        foreach (BurnSource burnSource in toRemove)
        {
            sources.Remove(burnSource);
        }

        return highestDamage;
    }
}

public class BurnSource
{
    public float duration;
    public float damage;

    public BurnSource(float damage, float duration)
    {
        this.duration = duration;
        this.damage = damage;
    }
}