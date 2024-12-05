using System.Linq;
using UnityEngine;

public class FireworkExplosion : MonoBehaviour
{
    [SerializeField] AudioClip explosionSound;
    public float dmg;
    public bool canSpawnMini;

    public float minTime;

    private int explosions = 1;
    private float totalTime;
    public PlayerAbility abilitySource;
    public void OnEnable()
    {
        transform.localScale = Vector3.one;
        AudioController.PlaySound(explosionSound);
        explosions = Random.Range(1, 3);
        totalTime = 1f;
    }

    private void Update()
    {

        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
        }
        else
        {
            PoolManager.Return(gameObject, GetType());
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            bool isCritical = abilitySource.GetCritChance() > Random.Range(0f, 100f);
            if (isCritical) dmg *= 2.5f;

            enemy.TakeDamage(dmg, isCritical);
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}