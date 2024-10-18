using UnityEngine;

public class LunarRainRay : MonoBehaviour
{
    public float cooldown;
    public float dmg;
    int level;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioClip sound;
    float alpha;
    public void OnEnable()
    {
        level = (int)Player.instance.abilityValues["ability.lunarrain.level"];

        dmg = level < 4 ? level < 2 ? 15f : 30f : 50f;

        AudioController.PlaySound(sound, Random.Range(0.8f, 1.2f));
    }

    public void OnAnimationEnd()
    {
        PoolManager.Return(gameObject, GetType());
    }

    public void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = Player.instance.currentStats.Atk * dmg;
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            bool damageAnotherEnemy = false;
            if ((int)Player.instance.abilityValues["ability.lunarrain.level"] >= 7)
            {
                if (enemy.CurrentHP - damage <= 0)
                {
                    damageAnotherEnemy = true;
                }
            }
            
            enemy.TakeDamage((int)damage, isCritical);

            if (damageAnotherEnemy)
            {
                Enemy e = Map.GetRandomClosebyEnemy();
                if (e == null) return;

                LunarRainRay ray = PoolManager.Get<LunarRainRay>();
                ray.transform.position = e.transform.position;
            }
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}