using UnityEngine;

public class SanctuaryCross : MonoBehaviour
{
    float speed;
    float angle;
    float dmg;
    public PlayerAbility abilitySource;

    private void OnEnable()
    {
        abilitySource = Player.instance.equippedPassiveAbilities.Find(x => x.GetType() == typeof(SanctuaryAbilityEvolution));
    }

    private void Update()
    {
        speed = 25 * abilitySource.itemValues["orbitalSpeed"];
        angle += speed * Time.deltaTime;
        if (angle > 360) angle -= 360;
        transform.localEulerAngles = new Vector3(0, 0, angle);
        dmg = 20;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = (int)(Player.instance.currentStats.Atk * dmg);
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;
            enemy.TakeDamage((int)damage, isCritical);

        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}