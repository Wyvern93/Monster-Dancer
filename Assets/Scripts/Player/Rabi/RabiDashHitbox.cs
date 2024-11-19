using UnityEngine;

public class RabiDashHitbox : MonoBehaviour
{
    public float dmg;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.illusiondash.level")) return;
       
        dmg = 60;

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = (int)(Player.instance.currentStats.Atk * dmg);
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) dmg *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)dmg, isCritical);

            Vector2 dir = enemy.transform.position - Player.instance.transform.position;
            enemy.PushEnemy(dir, 4f);
        }

        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            bullet.Despawn();
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}