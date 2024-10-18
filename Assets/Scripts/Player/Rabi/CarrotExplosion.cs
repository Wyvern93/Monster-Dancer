using System.Linq;
using UnityEngine;

public class CarrotExplosion : MonoBehaviour
{
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

            float damage = Player.instance.currentStats.Atk * dmg * Player.instance.itemValues["explosionDamage"];
            if (Player.instance.enhancements.Any(x => x.GetType() == typeof(DetonationCatalystItemEnhancement))) damage *= (Player.instance.itemValues["explosionSize"] * 1.5f) - 0.5f;

            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage(damage, isCritical);
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}