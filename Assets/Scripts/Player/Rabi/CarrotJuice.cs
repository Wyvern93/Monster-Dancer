using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CarrotJuice : MonoBehaviour, IDespawneable
{
    private float beats = 12;
    [SerializeField] AudioClip breakSound;

    List<Enemy> enemies;
    private float dmg = 1;
    private float targetSize;

    public void OnEnable()
    {
        int level = (int)Player.instance.abilityValues["ability.carrotjuice.level"];
        targetSize = level < 5 ? 1f : 1.5f;
        transform.localScale = Vector3.one * 0.1f;

        dmg = level < 6 ? level < 4 ? level < 2 ? 4f : 6f : 8f : 12f;

        enemies = new List<Enemy>();
        beats = 12;
        AudioController.PlaySound(breakSound, Random.Range(0.8f, 1.2f));
    }

    public void OnDespawn()
    {
        Player.instance.despawneables.Remove(this);
        PoolManager.Return(gameObject, GetType());
    }

    public void Update()
    {
        if (GameManager.isPaused) return;
        if (BeatManager.isQuarterBeat)
        {
            beats -= 0.25f;
            if (beats <= 0) targetSize = 0;
        }

        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * targetSize, Time.deltaTime * 4f);
        if (transform.localScale.magnitude <= 0.1f) OnDespawn();

        if (beats <= 0) return;

        if (BeatManager.isQuarterBeat)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                float damage = Player.instance.currentStats.Atk * dmg;
                if (damage < 1) damage = 1;
                bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
                if (isCritical) damage *= Player.instance.currentStats.CritDmg;
                if (enemies[i].CanBeSlowed(false)) enemies[i].OnSlow(1, 0.5f);
                enemies[i].TakeDamage((int)damage, isCritical);
            }
        }
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

    public void ForceDespawn(bool instant = false)
    {
        StopAllCoroutines();
        if (instant) PoolManager.Return(gameObject, GetType());
        else OnDespawn();

    }
}