using UnityEngine;

public class LunarPulse : MonoBehaviour
{
    public float cooldown;
    public float size;
    public float dmg;
    int level;

    [SerializeField] SpriteRenderer spriteRenderer;
    float alpha;
    public void OnEnable()
    {
        alpha = 0.5f;
        spriteRenderer.color = new Color(1,1,1, alpha);
        transform.localScale = Vector3.one * 0.5f;
        level = (int)Player.instance.abilityValues["ability.lunarpulse.level"];

        dmg = level < 4 ? 10f : 15f;
        size = level < 2 ? 2f : 4f;
    }

    public void Update()
    {
        if (transform.localScale.magnitude < size)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * size, size * Time.deltaTime);
            alpha = Mathf.MoveTowards(alpha, 0f, Time.deltaTime * 1.5f);
            Debug.Log(transform.localScale.magnitude);
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
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}