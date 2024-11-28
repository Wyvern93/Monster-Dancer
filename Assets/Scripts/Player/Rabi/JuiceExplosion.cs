using System.Linq;
using UnityEngine;

public class JuiceExplosion : MonoBehaviour
{
    public float dmg;
    public CarrotJuiceAbility abilitySource;

    public void OnFinish()
    {
        PoolManager.Return(gameObject, GetType());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = abilitySource.GetSplashDamage();

            bool isCritical = abilitySource.GetCritChance() > Random.Range(0f, 100f);
            if (isCritical) dmg *= 2.5f;

            enemy.TakeDamage(damage, isCritical);
            foreach (PlayerItem item in abilitySource.equippedItems)
            {
                if (item == null) continue;
                item.OnHit(abilitySource, dmg, enemy);
            }
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}