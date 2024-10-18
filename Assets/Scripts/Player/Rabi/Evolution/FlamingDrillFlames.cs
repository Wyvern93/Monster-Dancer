using UnityEngine;

public class FlamingDrillFlames : MonoBehaviour
{
    public float time;

    private void OnEnable()
    {
        time = 1f;
    }
    private void Update()
    {
        if (time > 0) time -= Time.deltaTime;
        else PoolManager.Return(gameObject, GetType());
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = (int)(Player.instance.currentStats.Atk * 25f);
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)damage, isCritical);
            foreach (PlayerItem item in Player.instance.equippedItems)
            {
                item.OnHit(Player.instance.equippedPassiveAbilities.Find(x => x.GetType() == typeof(FlamingDrillAbilityEvolution)), damage, enemy);
            }
            foreach (PlayerItem item in Player.instance.evolvedItems)
            {
                item.OnHit(Player.instance.equippedPassiveAbilities.Find(x => x.GetType() == typeof(FlamingDrillAbilityEvolution)), damage, enemy);
            }
            enemy.OnBurn();
        }
    }
}