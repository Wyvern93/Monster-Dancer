public class ArchetypeStats
{
    public float baseHP;
    public float baseSpeed;
    public float baseAttack;
    public EnemyArchetype archetypeType;

    public ArchetypeStats(EnemyArchetype archetype)
    {
        this.archetypeType = archetype;
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
                baseHP = 300f;
                baseSpeed = 0.8f;
                baseAttack = 10f;
                break;
            case EnemyArchetype.Boss:
                baseHP = 5000f;
                baseSpeed = 1f;
                baseAttack = 12f;
                break;
        }
    }

    public ArchetypeStats getStatsAtWave(int wave)
    {
        ArchetypeStats finalStats = new ArchetypeStats(this.archetypeType);
        finalStats.baseHP = baseHP;
        finalStats.baseSpeed = baseSpeed;
        finalStats.baseAttack = baseAttack;

        if (archetypeType == EnemyArchetype.Elite)
        {
            finalStats.baseHP += (baseHP * (wave * 0.15f)) * (Stage.Instance.elitesDefeated + 1);
        }
        else
        {
            finalStats.baseHP += baseHP * (wave * 0.15f);
        }

        
        finalStats.baseSpeed += baseSpeed * (wave * 0.003f);
        finalStats.baseAttack += baseAttack * (wave * 0.04f);

        return finalStats;
    }
}