using UnityEngine;

public class CarrotBullet : MonoBehaviour, IDespawneable
{
    public float dmg;

    Vector2 dir;
    public bool isPiercing;

    public PlayerAbility abilitySource;

    float lifeTime;
    public void OnEnable()
    {
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
        if (dir == Vector2.zero || lifeTime < 0)
        {
            Player.instance.despawneables.Remove(this);
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

            enemy.TakeDamage((int)dmg, isCritical);
            foreach (PlayerItem item in abilitySource.equippedItems)
            {
                if (item == null) continue;
                item.OnHit(abilitySource, dmg, enemy);
            }
            if (!isPiercing)
            {
                Player.instance.despawneables.Remove(this);
                PoolManager.Return(gameObject, GetType());
            }
        }
    }

    public void ForceDespawn(bool instant = false)
    {
        PoolManager.Return(gameObject, GetType());
    }
}