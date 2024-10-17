using System.Linq;
using UnityEngine;

public class SanctuaryWave : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    float alpha;
    [SerializeField] CircleCollider2D circleCollider;

    float heal;
    int bullets;

    private void OnEnable()
    {
        alpha = 0;
        transform.localScale = Vector3.zero;
        circleCollider.enabled = false;
        sprite.color = new Color(1, 1, 1, alpha);
        bullets = 0;
    }

    public void Cast()
    {
        alpha = 1;
        circleCollider.enabled = true;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * 2f, Time.deltaTime * 4f);
        alpha = Mathf.MoveTowards(alpha, 0, Time.deltaTime * 2);
        circleCollider.enabled = alpha > 0.1f;
        sprite.color = new Color(1, 1, 1, alpha);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            AudioController.PlaySound(AudioController.instance.sounds.superGrazeSound);

            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet == null) return;
            if (bullet.beat <= 0) return;

            int chance = 100;
            if (bullet.enemySource is Boss) chance = 10;
            else chance = 30;
            if (Random.Range(0, 100) < chance)
            {
                HolyOrb orb = PoolManager.Get<HolyOrb>();
                orb.transform.position = bullet.transform.position;
            }
            bullets++;
            
            bullet.ForceDespawn();
        }
    }
}