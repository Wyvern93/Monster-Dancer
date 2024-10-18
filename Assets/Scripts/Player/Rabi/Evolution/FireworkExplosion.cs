using System.Linq;
using UnityEngine;

public class FireworkExplosion : MonoBehaviour
{
    [SerializeField] AudioClip explosionSound;
    public float dmg;
    public bool canSpawnMini;

    public float minTime;

    private int explosions = 3;
    private float totalTime;
    public void OnEnable()
    {
        transform.localScale = Vector3.one;
        AudioController.PlaySound(explosionSound);
        explosions = Random.Range(1, 3);
        minTime = 0.3f;
        totalTime = 1f;
    }

    private void Update()
    {
        if (minTime > 0)
        {
            minTime -= Time.deltaTime;
        }
        else
        {
            if (explosions > 0) SpawnMiniExplosion();
            minTime = 0.3f;
        }

        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
        }
        else
        {
            PoolManager.Return(gameObject, GetType());
        }
    }

    private void SpawnMiniExplosion()
    {
        if (!canSpawnMini) return;
        explosions--;
        FireworkExplosion explosion = PoolManager.Get<FireworkExplosion>();
        explosion.dmg = dmg * 0.5f;
        explosion.canSpawnMini = false;
        explosion.transform.localScale = Vector3.one * 0.5f;
        explosion.transform.position = transform.position + (Vector3)((Random.insideUnitCircle.normalized));
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