using UnityEngine;

public class TestingStage : Stage
{
    protected override void Awake()
    {
        Instance = this;
        currentWave = 0;
        StageTime = 0;
        beats = beatsBeforeWave - 1;
        canSpawnEnemies = true;
    }

    protected override void Update()
    {
        if (!GameManager.isPaused) spawnAngle += Time.deltaTime * 30;
    }
}