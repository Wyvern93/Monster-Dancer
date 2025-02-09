using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    public EnemyAIType aIType;
    public bool centerIsLead;

    public List<Enemy> enemies;
    public Vector2 dirToPlayer;
    bool isInitialized;
    public float orbitDistance;
    public bool clockwise;
    public float orbitAngle;
    public float orbitSpeed;
    public int totalEnemies;

    private void OnEnable()
    {
        isInitialized = false;
    }
    public void OnGroupInit()
    {
        switch (aIType)
        {
            case EnemyAIType.Spread:
                dirToPlayer = Vector2.zero;
                break;
            case EnemyAIType.HordeChase:
            case EnemyAIType.CircleHorde:
                Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);
                dirToPlayer = ((Vector2)playerPos - GetCenter()).normalized;
                break;
            case EnemyAIType.Orbital:
                clockwise = true;
                break;

        }
        isInitialized = true;
        StartCoroutine(handlePhysics());
    }

    private IEnumerator handlePhysics()
    {
        switch (aIType)
        {
            case EnemyAIType.Spread:
                foreach (Enemy enemy in enemies)
                {
                    enemy.circleCollider.enabled = true;
                }
                break;
            case EnemyAIType.Orbital:
                foreach (Enemy enemy in enemies)
                {
                    enemy.circleCollider.enabled = false;
                }
                break;
            case EnemyAIType.CircleHorde:
            case EnemyAIType.HordeChase:
            case EnemyAIType.FullHorizontalWall:
            case EnemyAIType.FullVerticalWall:
            case EnemyAIType.HorizontalWall:
            case EnemyAIType.VerticalWall:
                foreach (Enemy enemy in enemies)
                {
                    enemy.circleCollider.enabled = true;
                }
                yield return new WaitForSeconds(0.3f);
                foreach (Enemy enemy in enemies)
                {
                    enemy.circleCollider.enabled = false;
                }
                break;
        }
        yield break;
    }

    void Respawn()
    {
        StartCoroutine(handlePhysics());
        switch (aIType)
        {
            case EnemyAIType.CircleHorde:
                Stage.RespawnHorde(this);
                Vector3 playerPos = Player.instance.transform.position;
                dirToPlayer = ((Vector2)playerPos - GetCenter()).normalized;
                break;
        }
    }
    void Update()
    {
        if (!isInitialized) return;
        if (enemies.Count == 0)
        {
            if (aIType == EnemyAIType.Orbital) Stage.Instance.currentOrbitalEvents--;
            PoolManager.Return(gameObject, GetType());
            return;
        }

        Vector3 playerPos = Player.instance.GetClosestPlayer(transform.position);

        switch (aIType)
        {
            case EnemyAIType.Spread:
                dirToPlayer = ((Vector2)playerPos - GetCenter()).normalized;
                break;
            case EnemyAIType.HordeChase:
                dirToPlayer = ((Vector2)playerPos - GetCenter()).normalized;
                break;
            case EnemyAIType.Orbital:
                dirToPlayer = ((Vector2)playerPos - GetCenter()).normalized;
                if (BeatManager.isBeat) StartCoroutine(OrbitalUpdate());
                break;
            case EnemyAIType.CircleHorde:
                if (Mathf.Abs(GetCenter().x - PlayerCamera.instance.transform.position.x) > 15) Respawn();
                if (Mathf.Abs(GetCenter().y - PlayerCamera.instance.transform.position.y) > 10) Respawn();// Out of Screen
                break;
        }
    }

    private IEnumerator OrbitalUpdate()
    {
        float beatDuration = BeatManager.GetBeatDuration() / 1.5f;
        float beatTime = 1;
        float time = 0;
        while (time <= BeatManager.GetBeatDuration() / 1.5f)
        {
            while (GameManager.isPaused) yield return null;
            float beatProgress = time / beatDuration;
            beatTime = Mathf.SmoothStep(1, 0f, beatProgress);

            if (clockwise) orbitAngle += 90f * (orbitSpeed / orbitDistance) * Time.deltaTime;
            else orbitAngle -= 90f * (orbitSpeed / orbitDistance) * Time.deltaTime;

            dirToPlayer = (Player.instance.transform.position - transform.position).normalized;

            transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, 5f * orbitSpeed * Time.deltaTime);
            time += Time.deltaTime;

            yield return null;
        }
        yield break;
    }

    public Vector2 GetCenter()
    {
        if (aIType == EnemyAIType.Orbital) return transform.position;
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