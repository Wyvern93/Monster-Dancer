using UnityEngine;

public class CarrotExplosion : MonoBehaviour
{
    [SerializeField] AudioClip explosionSound;

    public void OnEnable()
    {
        Player.TriggerCameraShake(0.2f, 0.2f);
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
            enemy.TakeDamage(Player.instance.currentStats.Atk * 12);
        }
    }
}