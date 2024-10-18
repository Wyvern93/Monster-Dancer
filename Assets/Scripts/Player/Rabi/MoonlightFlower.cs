using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonlightFlower : MonoBehaviour, IDespawneable
{
    [SerializeField] SpriteRenderer sprite;
    Color color = Color.white;
    public int beats;
    public float dmg;
    public float angle;
    public int level;
    private Transform playerTransform;
    private List<Enemy> enemies = new List<Enemy>();
    private float cd;
    private float speed;

    public void OnEnable()
    {
        enemies.Clear();
        GetComponent<Animator>().speed = 1f / BeatManager.GetBeatDuration();
        level = (int)Player.instance.abilityValues["ability.moonlightflower.level"];
        color.a = 1;
        beats = level < 3 ? 6 : 12;
        sprite.color = color;
        dmg = level < 4 ? level < 2 ? 3f : 6f : 12f;
        dmg *= Player.instance.itemValues["orbitalDamage"];
        playerTransform = Player.instance.transform;
        cd = 0;
        color.a = 1;
        speed = 50 * Player.instance.itemValues["orbitalSpeed"];
    }

    public void OnSpawn()
    {
        playerTransform = Player.instance.transform;
        transform.position = new Vector3(playerTransform.position.x + (8 * Mathf.Cos(angle * Mathf.Deg2Rad)), playerTransform.position.y + (4f * Mathf.Sin(angle * Mathf.Deg2Rad)), 2f);
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - (Time.deltaTime * 100));
    }

    public void Update()
    {
        if (BeatManager.isGameBeat)
        {
            if (beats > 0) color.a = 1;
            beats--;
        }
        if (color.a > 0) color.a -= Time.deltaTime;
        sprite.color = color;

        angle -= Time.deltaTime * speed;
        transform.position = new Vector3(playerTransform.position.x + (8 * Mathf.Cos(angle * Mathf.Deg2Rad)), playerTransform.position.y + (4f * Mathf.Sin(angle * Mathf.Deg2Rad)), 2f);
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - (Time.deltaTime * 100));

        if (cd <= 0)
        {
            cd = BeatManager.GetBeatDuration() / 4f;
            for (int i = 0; i < enemies.Count; i++)
            {
                float damage = (int)(Player.instance.currentStats.Atk * dmg);
                bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
                if (isCritical) damage *= Player.instance.currentStats.CritDmg;

                if (level >= 7)
                {
                    Vector2 dir = enemies[i].transform.position - Player.instance.transform.position;
                    enemies[i].PushEnemy(dir, 1f);
                }
                enemies[i].TakeDamage((int)damage, isCritical);
                
            }
        }
        else
        {
            cd -= Time.deltaTime;
        }

        if (color.a <= 0 && beats <= 0)
        {
            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, typeof(MoonlightFlower));
        }
    }

    private void OnDespawn()
    {
        beats = 0;
    }

    public void ForceDespawn(bool instant = false)
    {
        if (instant) PoolManager.Return(gameObject, GetType());
        else OnDespawn();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemies.Add(collision.GetComponent<Enemy>());
            Enemy enemy = collision.GetComponent<Enemy>();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (enemies.Contains(collision.GetComponent<Enemy>()))
            {
                enemies.Remove(collision.GetComponent<Enemy>());
            }
        }
    }
}