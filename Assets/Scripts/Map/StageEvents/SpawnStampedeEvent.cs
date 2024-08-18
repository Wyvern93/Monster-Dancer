using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SpawnStampedeEvent : StageTimeEvent
{
    SpawnData data;
    int number;
    public SpawnStampedeEvent(SpawnData data, int number, float time) : base(time)
    {
        this.data = data;
        this.number = number;
    }

    public override void Trigger()
    {
        float angle, x, y;
        angle = Random.Range(0, 360f);
        x = Player.instance.transform.position.x + (15 * Mathf.Cos(angle));
        y = Player.instance.transform.position.y + (15 * Mathf.Sin(angle));

        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

        Vector3 playerPos = Player.instance.GetClosestPlayer(spawnPos) + (Vector3)Random.insideUnitCircle * 2;
        Vector2 dir = playerPos - spawnPos;
        dir.Normalize();

        Enemy enemy = Enemy.GetEnemyOfType(data.enemyType);
        enemy.AItype = data.AItype;
        enemy.SpawnIndex = 0;


        float size = 1.5f;
        for (int i = 0; i < number; i++)
        {
            Vector3 random = (Vector3)(Random.insideUnitCircle * size) + spawnPos;
            Enemy e = Enemy.GetEnemyOfType(data.enemyType);
            e.transform.position = random;
            e.eventMove = dir;
            e.OnSpawn();
        }
    }
}