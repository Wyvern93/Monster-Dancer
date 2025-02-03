using System.Collections.Generic;
using UnityEngine;

public class CarrotJuice : MonoBehaviour, IDespawneable
{
    public float duration = 12;
    [SerializeField] AudioClip breakSound;
    public CarrotJuiceAbility abilitySource;

    List<Enemy> enemies;
    public float dmg = 1;
    public float targetSize;

    public void OnEnable()
    {
        transform.localScale = Vector3.one * 0.1f;

        enemies = new List<Enemy>();
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
            duration -= 0.25f;
            if (duration <= 0) targetSize = 0;
        }

        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * targetSize, Time.deltaTime * 4f);
        if (transform.localScale.magnitude <= 0.1f) OnDespawn();

        if (duration <= 0) return;

        if (BeatManager.isQuarterBeat)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                
                bool isCritical = abilitySource.GetCritChance() > Random.Range(0f, 100f);
                if (dmg < 1) dmg = 1;

                if (enemies[i].CanBeSlowed(false)) enemies[i].OnSlow(1, abilitySource.GetSlow());
                enemies[i].TakeDamage(isCritical ? dmg * 2.5f : dmg, isCritical);

                foreach (PlayerItem item in abilitySource.equippedItems)
                {
                    if (item == null) continue;
                    item.OnHit(abilitySource, dmg, enemies[i], isCritical);
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemies.Add(collision.GetComponent<Enemy>());
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
        PoolManager.Return(gameObject, GetType());
    }
}