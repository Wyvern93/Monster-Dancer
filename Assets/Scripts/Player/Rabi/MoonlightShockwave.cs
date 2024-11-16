using UnityEngine;

public class MoonlightShockwave : MonoBehaviour
{
    public float dmg;
    [SerializeField] AudioClip sfx;

    [SerializeField] SpriteRenderer spriteRenderer;
    float alpha;
    public void OnEnable()
    {
        alpha = 0.5f;
        spriteRenderer.color = new Color(1,1,1, alpha);

        dmg = 8;
        AudioController.PlaySound(sfx);
    }

    public void Update()
    {
        if (GameManager.isPaused) return;
        if (alpha > 0)
        {
            alpha = Mathf.MoveTowards(alpha, 0f, Time.deltaTime);
        }
        spriteRenderer.color = new Color(1, 1, 1, alpha);
        if (alpha <= 0)
        {
            alpha = 1;
            PoolManager.Return(gameObject, GetType());
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            dmg = 8;
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