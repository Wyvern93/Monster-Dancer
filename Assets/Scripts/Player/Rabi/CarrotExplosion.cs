using UnityEngine;

public class CarrotExplosion : MonoBehaviour
{
    [SerializeField] AudioClip explosionSound;
    public float dmg;
    public void OnEnable()
    {
        Player.TriggerCameraShake(0.3f, 0.2f);
        AudioController.PlaySound(explosionSound);
    }
    public void OnAnimationEnd()
    {
        PoolManager.Return(gameObject, GetType());
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