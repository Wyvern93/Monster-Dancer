using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    public EnemyAIType aIType;
    public bool centerIsLead;

    public List<Enemy> enemies;
    public Vector2 dirToPlayer;
    bool isInitialized;

    private void OnEnable()
    {
        isInitialized = false;
    }
    public void OnGroupInit()
    {
        switch (aIType)
        {
            case EnemyAIType.CircleHorde:
                Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
                dirToPlayer = ((Vector2)playerPos - GetCenter()).normalized;
                break;
        }
        isInitialized = true;
    }

    void Respawn()
    {
        switch (aIType)
        {
            case EnemyAIType.CircleHorde:
                float angle, x, y;
                angle = Random.Range(0, 360f);
                
                Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
                x = playerPos.x + (14 * Mathf.Cos(angle));
                y = playerPos.y + (14 * Mathf.Sin(angle));

                Vector3 spawnPos = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

                float size = 1.5f;
                foreach (Enemy e in enemies)
                {
                    if (e.isDead || !e.gameObject.activeSelf) continue;

                    Vector3 random = spawnPos + (Vector3)(Random.insideUnitCircle * 0.05f);//(Vector3)(Random.insideUnitCircle * size) + spawnPos;
                    e.transform.position = random;
                }
                dirToPlayer = ((Vector2)playerPos - GetCenter()).normalized;
                break;
        }
    }
    void Update()
    {
        if (!isInitialized) return;
        if (enemies.Count == 0)
        {
            PoolManager.Return(gameObject, GetType());
            return;
        }
        switch (aIType)
        {
            case EnemyAIType.Spread:
                Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
                dirToPlayer = ((Vector2)playerPos - GetCenter()).normalized;
                break;
            case EnemyAIType.CircleHorde:
                if (Vector2.Distance(GetCenter(), Player.instance.transform.position) > 15) Respawn();        // Out of Screen
                break;
        }
    }

    public Vector2 GetCenter()
    {
        if (!centerIsLead)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy == null) continue;
                if (enemy.isDead) continue;
                if (!enemy.gameObject.activeSelf) continue;
                return enemy.transform.position;
            }
        }
        Vector3 pos = Vector2.zero;
        int count = 0;
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            if (enemy.isDead) continue;
            if (!enemy.gameObject.activeSelf) continue;
            pos += enemy.transform.position;
            count++;
        }
        pos /= count;
        return pos;
    }
}