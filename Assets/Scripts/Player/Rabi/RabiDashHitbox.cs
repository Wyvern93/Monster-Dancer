using UnityEngine;

public class RabiDashHitbox : MonoBehaviour
{
    public float dmg;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.illusiondash.level")) return;
        
        int level = (int)Player.instance.abilityValues["ability.illusiondash.level"];
        dmg = level < 3 ? 60 : 120;

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = (int)(Player.instance.currentStats.Atk * dmg);
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) dmg *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)dmg, isCritical);
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}