using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CarrotJuice : MonoBehaviour, IDespawneable
{
    private int beats = 16;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip breakSound;

    private float cd;

    List<Enemy> enemies;
    private float dmg = 1;

    public void OnEnable()
    {
        int level = (int)Player.instance.abilityValues["ability.carrotjuice.level"];
        transform.localScale = Vector3.one * (level < 5 ? 1f : 1.5f);

        dmg = level < 6 ? level < 4 ? level < 2 ? 4f : 6f : 8f : 12f;

        animator.Play("CarrotJuice_Spawn");
        enemies = new List<Enemy>();
        cd = 0;
        beats = 12;
        AudioController.PlaySound(breakSound, Random.Range(0.8f, 1.2f));
    }
    public void OnAnimationEnd()
    {
        Player.instance.despawneables.Remove(this);
        PoolManager.Return(gameObject, GetType());
    }

    public void OnDespawn()
    {
        animator.Play("CarrotJuice_Despawn");
    }

    public void Update()
    {
        if (GameManager.isPaused) return;
        if (BeatManager.isGameBeat)
        {
            beats--;
            if (beats == 0)
            {
                OnDespawn();
            }
        }

        if (beats <= 0) return;

        if (cd <= 0)
        {
            cd = BeatManager.GetBeatDuration() / 4f;
            for (int i = 0; i < enemies.Count; i++)
            {
                float damage = Player.instance.currentStats.Atk * dmg;
                if (damage < 1) damage = 1;
                bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
                if (isCritical) damage *= Player.instance.currentStats.CritDmg;

                enemies[i].TakeDamage((int)damage, isCritical);
            }
        }
        else
        {
            cd -= Time.deltaTime;
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