using System;

[Serializable]
public class StageWaveEnemyData
{
    public EnemyType main_runner, secondary_runner, third_runner;
    public EnemyType main_bomber, secondary_bomber;
    public EnemyType main_shooter, secondary_shooter;

    public EnemyType specialSpawnEnemy;
}