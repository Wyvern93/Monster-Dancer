using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public EnemyType enemyType;
    public EnemyClass enemyClass;
    public EnemyArchetype archetype;
    public List<StageEventType> allowedEvents;

    ArchetypeStats baseStats;

    public EnemyData(EnemyType enemyType, EnemyClass enemyClass, EnemyArchetype archetype, List<StageEventType> allowedEvents)
    {
        this.enemyType = enemyType;
        this.enemyClass = enemyClass;
        this.archetype = archetype;
        this.allowedEvents = allowedEvents;

        baseStats = new ArchetypeStats(archetype);
    }

    public int CalculateExperience(int wave)
    {

        float GlobalExpMultiplier = 0.07f;  // Global EXP multiplier
        float CurveMultiplier = 1.2f;     // Curve steepness
        ArchetypeStats baseStats = new ArchetypeStats(archetype); //.getStatsAtWave(wave);

        float MaxHP = baseStats.baseHP;
        float Speed = baseStats.baseSpeed;
        float Attack = baseStats.baseAttack;

        float baseExp = (MaxHP * 0.3f) + (Attack * 0.3f);
        float typeMultiplier = enemyClass switch
        {
            EnemyClass.Elite => 5.0f,
            EnemyClass.Shooter => 1.5f,
            EnemyClass.Bomber => 1.25f,
            EnemyClass.Runner => 1.0f,
            EnemyClass.Boss => 0f,
            _ => 1.0f,
        };
        baseExp *= Mathf.Pow(1 + CurveMultiplier, 1 + (wave / 12f));
        baseExp *= GlobalExpMultiplier;

        return Mathf.RoundToInt(baseExp * typeMultiplier);
    }
} 