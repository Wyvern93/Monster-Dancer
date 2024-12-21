public class ArchetypeStats
{
    public float baseHP;
    public float baseSpeed;
    public float baseAttack;

    public ArchetypeStats(EnemyArchetype archetype)
    {
        float defaultHP = 30;
        switch (archetype)
        {
            default:
            case EnemyArchetype.AllRounder:
                baseHP = defaultHP * 1f;
                baseSpeed = 0.6f;
                baseAttack = 5f;
                break;
            case EnemyArchetype.Swarm:
                baseHP = defaultHP * 0.4f;
                baseSpeed = 1.2f;
                baseAttack = 2f;
                break;
            case EnemyArchetype.Menacer:
                baseHP = defaultHP * 2.5f;
                baseSpeed = 1.0f;
                baseAttack = 2f;
                break;
            case EnemyArchetype.Glasscannon:
                baseHP = defaultHP * 0.4f;
                baseSpeed = 0.45f;
                baseAttack = 10f;
                break;
            case EnemyArchetype.Tank:
                baseHP = defaultHP * 3.5f;
                baseSpeed = 0.35f;
                baseAttack = 2f;
                break;
            case EnemyArchetype.Juggernaut:
                baseHP = defaultHP * 2.5f;
                baseSpeed = 0.45f;
                baseAttack = 8f;
                break;
            case EnemyArchetype.Rusher:
                baseHP = defaultHP * 0.45f;
                baseSpeed = 1.1f;
                baseAttack = 8f;
                break;
            case EnemyArchetype.Dangerous:
                baseHP = defaultHP * 3f;
                baseSpeed = 1.1f;
                baseAttack = 8f;
                break;
            case EnemyArchetype.Elite:
                baseHP = 600f;
                baseSpeed = 1f;
                baseAttack = 10f;
                break;
            case EnemyArchetype.Boss:
                baseHP = 6000f;
                baseSpeed = 1f;
                baseAttack = 12f;
                break;
        }
    }

    public ArchetypeStats getStatsAtWave(int wave)
    {
        ArchetypeStats finalStats = new ArchetypeStats(EnemyArchetype.AllRounder);
        finalStats.baseHP = baseHP;
        finalStats.baseSpeed = baseSpeed;
        finalStats.baseAttack = baseAttack;

        finalStats.baseHP += baseHP * (wave * 0.2f);
        finalStats.baseSpeed += baseSpeed * (wave * 0.003f);
        finalStats.baseAttack += baseAttack * (wave * 0.04f);

        return finalStats;
    }
}