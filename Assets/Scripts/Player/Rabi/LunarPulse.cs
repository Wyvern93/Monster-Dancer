using Unity.Burst.CompilerServices;
using UnityEngine;

public class LunarPulse : MonoBehaviour
{
    public float cooldown;
    public float size;
    public float dmg;

    [SerializeField] SpriteRenderer spriteRenderer;
    float alpha;
    public void OnEnable()
    {
        alpha = 0.5f;
        spriteRenderer.color = new Color(1,1,1, alpha);
        transform.localScale = Vector3.one * 0.5f;
        size =  6f;
    }

    public void Update()
    {
        if (GameManager.isPaused) return;
        if (transform.localScale.magnitude < size)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * size, size * Time.deltaTime * 2f);
            if (transform.localScale.magnitude > size - 0.5f)
            {
                alpha = Mathf.MoveTowards(alpha, 0f, Time.deltaTime);
            }
            
        }
        spriteRenderer.color = new Color(1, 1, 1, alpha);
        if (transform.localScale.magnitude >= size)
        {
            alpha = 1;
            PoolManager.Return(gameObject, GetType());
        }
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 200 * Time.deltaTime);
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

            Vector2 dir = enemy.transform.position - Player.instance.transform.position;
            enemy.PushEnemy(dir, 2f);
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}