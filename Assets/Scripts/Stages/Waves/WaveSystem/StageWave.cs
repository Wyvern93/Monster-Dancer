using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageWave
{
    public List<StageEventType> allowedEventTypes;
    public StageWaveEnemyData waveData;
    

    public WavePreset choosenPreset;

    [HideInInspector] public bool isInitialized;
    [HideInInspector] public bool isFinalized;

    public void Initialize()
    {
        int currentWave = Stage.Instance.currentWave;

        choosenPreset = new WavePreset();
        choosenPreset.events = new List<StageEvent>();
        
        float difficultyRating = 1.2f + (currentWave / 18f); // 1.2f + (currentWave / 7f);
        float minimumDifficulty = difficultyRating;
        float maximumDifficulty = difficultyRating + 1.5f;

        // Calculate here what amount of runners and shooters for this wave, based on difficulty but also randomly, THIS IS YET TO BE DONE
        int runnerWaves = 0;
        int bomberWaves = 0;
        int shooterWaves = 0;

        int runnerEnemyTypes = waveData.third_runner == EnemyType.None ? waveData.secondary_runner == EnemyType.None ? waveData.main_runner == EnemyType.None ? 0 : 1 : 2 : 3;
        int bomberEnemyTypes = waveData.secondary_bomber == EnemyType.None ? waveData.main_bomber == EnemyType.None ? 0 : 1 : 2;
        int shooterEnemyTypes = waveData.secondary_shooter == EnemyType.None ? waveData.main_shooter == EnemyType.None ? 0 : 1 : 2;

        float tempDifficulty = 0;
        float attempts = 100;


        while (tempDifficulty < minimumDifficulty)
        {
            int choosenClass = UnityEngine.Random.Range(0, 10); // 0-5 Runner 6-8 Shooter 9-10 Bomber

            if (choosenClass < 6) choosenClass = 0; // Runners
            else if (choosenClass < 9) choosenClass = 2; // Shooter;
            else choosenClass = 1;

            if (choosenClass == 1 && tempDifficulty + 2 > maximumDifficulty) choosenClass = 0;
            else if (choosenClass == 2 && tempDifficulty + 2 > maximumDifficulty) choosenClass = 1;

            if (choosenClass == 0 && runnerEnemyTypes > 0)
            {
                runnerWaves++;
                tempDifficulty += 1;
            }
            else if (choosenClass == 1 && bomberEnemyTypes > 0)
            {
                bomberWaves++;
                tempDifficulty += 1.5f;
            }
            else if (choosenClass == 2 && shooterEnemyTypes > 0)
            {
                shooterWaves++;
                tempDifficulty += 1.25f;
            }

            // Make sure it is not stuck
            if (attempts <= 0)
            {
                break;
            }

            // Try to stay in the difficulty range
            if (tempDifficulty > maximumDifficulty)
            {
                tempDifficulty = 0;
                runnerWaves = 0;
                bomberWaves = 0;
                shooterWaves = 0;
                
            }
            attempts--;
        }

        int currentRunnerType = 0;
        int currentBomberType = 0;
        int currentShooterType = 0;

        // Class groups calculated, now look for the event groups with those numbers and make a loop like in the doc
        if (runnerWaves > 0)
        {
            int remainingWaves = runnerWaves;
            int attempts2 = 10;
            while (remainingWaves > 0)
            {
                int randomRange = UnityEngine.Random.Range(1, remainingWaves);

                EnemySpawnType choosenSpawnType;
                if (currentRunnerType == 0) choosenSpawnType = EnemySpawnType.MAIN_RUNNER;
                else if (currentRunnerType == 1) choosenSpawnType = EnemySpawnType.SECOND_RUNNER;
                else choosenSpawnType = EnemySpawnType.THIRD_RUNNER;

                EnemyType choosenEnemy = getEnemyFromSpawnType(choosenSpawnType);

                StageEventGroup runnerGroup = StageEventGroup.getStageEventGroup(choosenEnemy, randomRange);
                attempts2--;
                if (attempts2 <= 0) break;

                if (runnerGroup != null)
                {
                    remainingWaves -= randomRange;

                    for (int i = 0; i < randomRange; i++)
                    {
                        choosenPreset.events.Add(getEventFromGroup(runnerGroup, choosenSpawnType));
                        choosenPreset.events.Add(new WaitEvent(4));
                    }
                    currentRunnerType++;
                    if (currentRunnerType >= runnerEnemyTypes) currentRunnerType = 0;
                }
            }
        }
        
        if (shooterWaves > 0)
        {
            int remainingWaves = shooterWaves;
            while (remainingWaves > 0)
            {
                int randomRange = UnityEngine.Random.Range(1, remainingWaves);

                EnemySpawnType choosenSpawnType;
                if (currentShooterType == 0) choosenSpawnType = EnemySpawnType.MAIN_SHOOTER;
                else choosenSpawnType = EnemySpawnType.SECOND_SHOOTER;

                EnemyType choosenEnemy = getEnemyFromSpawnType(choosenSpawnType);

                StageEventGroup shooterGroup = StageEventGroup.getStageEventGroup(choosenEnemy, randomRange);

                if (shooterGroup != null)
                {
                    remainingWaves -= randomRange;

                    for (int i = 0; i < randomRange; i++)
                    {
                        choosenPreset.events.Add(getEventFromGroup(shooterGroup, choosenSpawnType));
                        choosenPreset.events.Add(new WaitEvent(8));
                    }
                    currentShooterType++;
                    if (currentShooterType >= shooterEnemyTypes) currentShooterType = 0;
                }
            }
        }

        if (bomberWaves > 0)
        {
            int remainingWaves = bomberWaves;
            while (remainingWaves > 0)
            {
                int randomRange = UnityEngine.Random.Range(1, remainingWaves);

                EnemySpawnType choosenSpawnType;
                if (currentBomberType == 0) choosenSpawnType = EnemySpawnType.MAIN_BOMBER;
                else choosenSpawnType = EnemySpawnType.SECOND_BOMBER;

                EnemyType choosenEnemy = getEnemyFromSpawnType(choosenSpawnType);

                StageEventGroup bomberGroup = StageEventGroup.getStageEventGroup(choosenEnemy, randomRange);

                if (bomberGroup != null)
                {
                    remainingWaves -= randomRange;

                    for (int i = 0; i < randomRange; i++)
                    {
                        choosenPreset.events.Add(getEventFromGroup(bomberGroup, choosenSpawnType));
                        choosenPreset.events.Add(new WaitEvent(4));
                    }
                    currentBomberType++;
                    if (currentBomberType >= bomberEnemyTypes) currentBomberType = 0;
                }
            }
        }
    }

    public void ForceStop()
    {
        Stage.Instance.StopCoroutine(Start());
        isFinalized = true;
    }

    public EnemyType getEnemyFromSpawnType(EnemySpawnType spawnType)
    {
        switch (spawnType)
        {
            default:
            case EnemySpawnType.MAIN_RUNNER: return waveData.main_runner;
            case EnemySpawnType.SECOND_RUNNER: return waveData.secondary_runner;
            case EnemySpawnType.THIRD_RUNNER: return waveData.third_runner;
            case EnemySpawnType.MAIN_BOMBER: return waveData.main_bomber;
            case EnemySpawnType.SECOND_BOMBER: return waveData.secondary_bomber;
            case EnemySpawnType.MAIN_SHOOTER: return waveData.main_shooter;
            case EnemySpawnType.SECOND_SHOOTER: return waveData.secondary_shooter;
            case EnemySpawnType.ELITE:
            case EnemySpawnType.BOSS:
                return waveData.specialSpawnEnemy;
        }
    }

    private StageEvent getEventFromGroup(StageEventGroup group, EnemySpawnType spawnType)
    {
        int enemyNum;
        EnemyClass enemyClass = Enemy.enemyClassFromSpawnType(spawnType);
        if (enemyClass == EnemyClass.Runner) enemyNum = group.defaultRunnerNumber + (int)Stage.Instance.additionalRunners;
        else if (enemyClass == EnemyClass.Bomber) enemyNum = group.defaultBomberNumber + (int)Stage.Instance.additionalBombers;
        else enemyNum = group.defaultShooterNumber + (int)Stage.Instance.additionalShooters;

        switch (group.StageEventType)
        {
            default:
            case StageEventType.SpawnSpreadGroup:
                return new SpawnSpreadEvent(enemyNum, spawnType);
            case StageEventType.SpawnGeometricEvent:
                return new SpawnGeometricEvent(enemyNum, spawnType);
            case StageEventType.SpawnCircleHordeGroup:
                return new SpawnCircleHordeEvent(enemyNum, spawnType);
            case StageEventType.SpawnHordeChaseEvent:
                return new SpawnHordeChaseEvent(enemyNum, spawnType);
            case StageEventType.SpawnElite:
                return new SpawnEliteEvent();
            case StageEventType.SpawnBoss:
                return new SpawnBossEvent();
        }
    }

    public IEnumerator Start()
    {
        isInitialized = true;
        AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
        yield return choosenPreset.PlayWave(this);
        yield return new WaitUntil(() => BeatManager.isBeat);
        isFinalized = true;
    }
}