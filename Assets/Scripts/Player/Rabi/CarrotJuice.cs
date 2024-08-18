using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CarrotJuice : MonoBehaviour
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
        if (level >= 3) transform.localScale = Vector3.one * 2f;
        else transform.localScale = Vector3.one;

        dmg = level < 4 ? level < 2 ? 4f : 6f : 9f;

        animator.Play("CarrotJuice_Spawn");
        enemies = new List<Enemy>();
        cd = 0;
        beats = 16;
        AudioController.PlaySound(breakSound, Random.Range(0.8f, 1.2f));
    }
    public void OnAnimationEnd()
    {
        PoolManager.Return(gameObject, GetType());
    }

    public void OnDespawn()
    {
        animator.Play("CarrotJuice_Despawn");
    }

    public void Update()
    {
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
            cd = 0.1f;
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
}