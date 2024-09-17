using UnityEngine;

public class CarrotBullet : MonoBehaviour
{
    int level, dmg;

    Vector2 dir;

    float lifeTime;
    public void OnEnable()
    {
        level = (int)Player.instance.abilityValues["ability.carrotdelivery.level"];
        dmg = level < 3 ? 20 : 30;
        lifeTime = 3f;
    }

    public void SetDirection(Vector2 direction)
    {
        float targetDir = Vector2.SignedAngle(Vector2.down, direction);
        transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, direction));
        dir = direction;
    }

    public void Update()
    {
        if (GameManager.isPaused) return;
        transform.position += (Vector3)dir * 30 * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if (dir == Vector2.zero || lifeTime < 0) PoolManager.Return(gameObject, GetType());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = Player.instance.currentStats.Atk * dmg;
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)damage, isCritical);
            PoolManager.Return(gameObject, GetType());
        }
    }
}