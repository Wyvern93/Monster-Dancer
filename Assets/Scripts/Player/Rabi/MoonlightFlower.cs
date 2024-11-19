using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonlightFlower : MonoBehaviour, IDespawneable
{
    public int beats;
    public float dmg;
    private List<Enemy> enemies = new List<Enemy>();
    private float speed;
    [SerializeField] AudioClip sfx;
    private Vector2 targetPos;
    private float scale = 0;

    public void OnEnable()
    {
        scale = 0.2f;
        transform.localScale = Vector3.one * scale;
        enemies.Clear();
        GetComponent<Animator>().speed = 1f / BeatManager.GetBeatDuration();
        beats = 6;
        dmg = 3f;
        dmg *= Player.instance.itemValues["orbitalDamage"];
        speed = 4 * Player.instance.itemValues["orbitalSpeed"];
        AudioController.PlaySound(sfx);
    }

    public void OnSpawn()
    {
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - (Time.deltaTime * 100));
    }
    private void MoveTowardsTarget()
    {
        float acceleration = GetAcceleration() * speed;
        if (GameManager.isPaused) return;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * acceleration);
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
    }
    private float GetAcceleration()
    {
        float currentDistance = Vector2.Distance(transform.position, targetPos) * 4f;
        currentDistance = Mathf.Clamp(currentDistance, 0, 1f);
        return currentDistance;
    }

    public void Update()
    {
        if (GameManager.isPaused) return;

        scale = Mathf.MoveTowards(scale, 1f, Time.deltaTime * 4f);
        transform.localScale = Vector3.one * scale;

        targetPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        MoveTowardsTarget();
        if (BeatManager.isGameBeat)
        {
            beats--;
        }
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - (Time.deltaTime * 100));

        if (BeatManager.isQuarterBeat)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                float damage = (int)(Player.instance.currentStats.Atk * dmg);
                bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
                if (isCritical) damage *= Player.instance.currentStats.CritDmg;
                enemies[i].TakeDamage((int)damage, isCritical);
            }
        }

        if (beats <= 0)
        {
            MoonlightShockwave shockwave = PoolManager.Get<MoonlightShockwave>();
            shockwave.dmg = 24;
            shockwave.transform.position = transform.position;
            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, typeof(MoonlightFlower));
        }
    }

    private void OnDespawn()
    {
        MoonlightShockwave shockwave = PoolManager.Get<MoonlightShockwave>();
        shockwave.dmg = 4;
        shockwave.transform.position = transform.position;
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