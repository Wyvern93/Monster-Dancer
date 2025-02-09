using System.Linq;
using UnityEngine;

public class CarrotExplosion : MonoBehaviour, IPlayerExplosion
{
    public PlayerAbility abilitySource;
    [SerializeField] AudioClip explosionSound;
    public float dmg;
    public void OnEnable()
    {
        transform.localScale = Vector3.one;
        AudioController.PlaySound(explosionSound);
    }
    public void OnAnimationEnd()
    {
        PoolManager.Return(gameObject, GetType());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            bool isCritical = abilitySource.GetCritChance() > Random.Range(0f, 100f);

            enemy.TakeDamage(isCritical ? dmg * 2.5f : dmg, isCritical);
            foreach (PlayerItem item in abilitySource.equippedItems)
            {
                if (item == null) continue;
                item.OnHit(abilitySource, dmg, enemy, isCritical);
            }
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}