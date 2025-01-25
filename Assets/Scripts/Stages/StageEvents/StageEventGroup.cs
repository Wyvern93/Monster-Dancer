using System;
using System.Collections.Generic;
using System.Linq;

public class StageEventGroup
{
    public StageEventType StageEventType;
    public int minEvents;
    public int maxEvents;
    public int defaultRunnerNumber;
    public int defaultBomberNumber;
    public int defaultShooterNumber;

    public static List<StageEventGroup> stageEventGroups = new List<StageEventGroup>()
    {
        // Horde Circle
        new StageEventGroup() { minEvents = 1, maxEvents = 3, StageEventType = StageEventType.SpawnCircleHordeGroup, defaultRunnerNumber = 8, defaultBomberNumber = 5, defaultShooterNumber = 4 },

        // Horde Chase
        new StageEventGroup() { minEvents = 1, maxEvents = 5, StageEventType = StageEventType.SpawnHordeChaseEvent, defaultRunnerNumber = 6, defaultBomberNumber = 3, defaultShooterNumber = 4 },

        // Spread
        new StageEventGroup() { minEvents = 1, maxEvents = 5, StageEventType = StageEventType.SpawnSpreadGroup, defaultRunnerNumber = 6, defaultBomberNumber = 3, defaultShooterNumber = 4 },

        // Geometric
        new StageEventGroup() { minEvents = 1, maxEvents = 1, StageEventType = StageEventType.SpawnGeometricEvent, defaultRunnerNumber = 8, defaultBomberNumber = 3, defaultShooterNumber = 3 }
    };

    public float getBaseDifficulty(EnemyType enemyType)
    {
        EnemyClass enemyClass = Enemy.enemyData[enemyType].enemyClass;
        return enemyClass == EnemyClass.Runner ? 1f : enemyClass == EnemyClass.Bomber ? 1.5f : 2.5f;
    }

    public static StageEventGroup getStageEventGroup(EnemyType enemyType, int number)
    {
        List<StageEventGroup> groups = new List<StageEventGroup>();
        for (int i = 0; i < stageEventGroups.Count; i++)
        {
            if (number < stageEventGroups[i].minEvents) continue;
            if (number > stageEventGroups[i].maxEvents) continue;
            if (!Enemy.enemyData[enemyType].allowedEvents.Contains(stageEventGroups[i].StageEventType)) continue;
            //if (stageEventGroups[i].enemyClass != enemyClass) continue;
            groups.Add(stageEventGroups[i]);
        }
        if (groups.Count == 0) return null;
        else return groups[UnityEngine.Random.Range(0, groups.Count)];
    }
}