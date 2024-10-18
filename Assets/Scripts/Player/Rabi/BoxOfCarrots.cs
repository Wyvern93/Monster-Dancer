using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class BoxOfCarrots : MonoBehaviour, IDespawneable
{
    int level;
    float dmg;
    float heal;

    public void ForceDespawn(bool instant = false)
    {
        PoolManager.Return(gameObject, GetType());
    }

    public void OnEnable()
    {
        level = (int)Player.instance.abilityValues["ability.boxofcarrots.level"];
        dmg = level < 6 ? level < 4 ? level < 2 ? 30 : 42 : 59 : 78;
        heal = level < 5 ? 0.05f : 0.10f;
    }

    public void Update()
    {
        if (Vector2.Distance(transform.position, Player.instance.transform.position) > 20)
        {
            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, GetType());
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = Player.instance.currentStats.Atk * dmg;
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)damage, isCritical);

            CarrotExplosion carrotExplosion = PoolManager.Get<CarrotExplosion>();
            carrotExplosion.transform.position = transform.position;
            carrotExplosion.dmg = dmg;
            carrotExplosion.transform.localScale = Vector3.one * 2f;
            carrotExplosion.transform.localScale = Vector3.one * Player.instance.itemValues["explosionSize"];

            if (Player.instance.enhancements.Any(x => x.GetType() == typeof(FireworksKitItemEnhancement))) // This is never removed even with evolutions
            {
                bool doExplosion = Random.Range(0, 20) <= 0;
                if (doExplosion)
                {
                    Vector2 explosionPos = transform.position + (Random.insideUnitSphere.normalized * 2f);

                    CarrotExplosion carrotExplosion2 = PoolManager.Get<CarrotExplosion>();
                    carrotExplosion2.transform.position = explosionPos;
                    carrotExplosion2.dmg = dmg * 0.5f;
                    carrotExplosion2.transform.localScale = Vector3.one * Player.instance.itemValues["explosionSize"];
                }
            }

            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, GetType());
        }

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            int healnumber = (int)(Player.instance.currentStats.MaxHP * heal);
            Player.instance.Heal(healnumber);
            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, GetType());
        }
    }
}