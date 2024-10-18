using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotCopter : MonoBehaviour, IDespawneable
{
    private Enemy currentTarget;
    private bool isAttacking;
    private bool isInRange;
    private Vector2 targetPosition;
    private int level;
    private float dmg;
    private int ammo;
    private float speed;
    private float currentDistance;
    private bool enemiesAround;
    [SerializeField] AudioClip shootSound;
    public void OnEnable()
    {
        currentTarget = null;
        isAttacking = false;
        speed = 1f;
    }
    public void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
        if (!BeatManager.isGameBeat || GameManager.isPaused) return;
        if (currentTarget == null)
        {
            SearchTarget();
            if (currentTarget == null)
            {
                StartCoroutine(MoveTowardsPlayer());
                return;
            }
        }
        if (!currentTarget.gameObject.activeSelf)
        {
            SearchTarget();
        }
        if (!enemiesAround)
        {
            StartCoroutine(MoveTowardsPlayer());
            return;
        }

        CheckIfInRange();
        if (isInRange)
        {

            if (!isAttacking) StartCoroutine(AttackCoroutine());

        }
        else
        {
            StartCoroutine(MoveTowardsTarget());
        }
    }

    private void SearchTarget()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(Player.instance.transform.position, Vector2.one * 14f, 0);
        List<Enemy> enemies = new List<Enemy>();
        foreach (Collider2D collider in cols)
        {
            //if (collider == null) continue;
            //if (collider.gameObject.activeSelf) continue;
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            if (enemy) enemies.Add(enemy);
        }
        bool hasEnemy = false;
        foreach (Enemy enemy in enemies)
        {
            if (enemy.isElite)
            {
                currentTarget = enemy;
                hasEnemy = true;
            }
            if (enemy is Boss)
            {
                currentTarget = enemy;
                hasEnemy = true;
            }
        }
        if (!hasEnemy && enemies.Count > 0)
        {
            Enemy e = enemies[0];
            float distance = 9999;
            Vector3 pos = Player.instance.transform.position;
            foreach (Enemy enemy in enemies)
            {
                float dist = Vector2.Distance(pos, enemy.transform.position);
                if (dist < distance)
                {
                    e = enemy;
                    distance = dist;
                }
            }
            currentTarget = e;
        }
        enemiesAround = enemies.Count > 0;
    }

    private void CheckIfInRange()
    {
        currentDistance = Vector2.Distance(transform.position - (Vector3.up * 4f), currentTarget.transform.position);
        isInRange = currentDistance < 4f;
    }

    private IEnumerator MoveTowardsTarget()
    {
        float time = 0;
        Vector3 targetPos = currentTarget.transform.position;
        Vector2 dir = (targetPos - transform.position).normalized;
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(transform.position, targetPos + (Vector3.up * 4f), Time.deltaTime * speed);
            transform.position = new Vector3(transform.position.x, transform.position.y, 10);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    private IEnumerator MoveTowardsPlayer()
    {
        float time = 0;
        Vector3 targetPos = Player.instance.transform.position;
        Vector2 dir = (targetPos - transform.position).normalized;
        while (time <= BeatManager.GetBeatDuration() / 2)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(transform.position, Player.instance.transform.position + (Vector3.up * 4f), Time.deltaTime * speed);
            transform.position = new Vector3(transform.position.x, transform.position.y, 10);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        UpdateStats();
        int current_ammo = ammo;
        float current_dmg = dmg;

        while (current_ammo > 0)
        {
            if (currentTarget == null)
            {
                current_ammo = 0;
            }
            else if (!currentTarget.gameObject.activeSelf)
            {
                current_ammo = 0;
            }
            else
            {
                current_ammo--;
                ShootCarrot();
                yield return new WaitForSeconds(BeatManager.GetBeatDuration() / 4f);
            }
        }
        isAttacking = false;
        currentTarget = null;
        yield break;
    }

    private void UpdateStats()
    {
        level = (int)Player.instance.abilityValues["ability.carrotcopter.level"];
        dmg = level < 4 ? level < 2 ? 3 : 6 : 12; // 200% 200%
        ammo = level < 5 ? 4 : 8; // 4->8
        speed = level < 6 ? level < 3 ? 1f : 1.5f : 2f; // 25%
        // Lvl 2 and 4 DMG+
        // Lvl 5 Ammo
        // So Lvl 3 and 6 should be speed
        // Lvl 7 makes the drone's piercing
    }


    public void ShootCarrot()
    {
        CarrotBullet carrot = PoolManager.Get<CarrotBullet>();
        carrot.transform.position = transform.position;
        carrot.isPiercing = level >= 7;
        carrot.dmg = dmg;
        carrot.abilitySource = Player.instance.equippedPassiveAbilities.Find(x => x.GetType() == typeof(CarrotcopterAbility));
        Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());

        Vector2 dir = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
        carrot.SetDirection(dir);

        AudioController.PlaySoundWithoutCooldown(shootSound, Random.Range(0.9f, 1.1f));
    }

    public void ForceDespawn(bool instant = false)
    {
        PoolManager.Return(gameObject, GetType());
    }
}