using UnityEngine;

public class GroundFire : MonoBehaviour
{
    public float time;

    private void OnEnable()
    {
        time = 2f;
    }
    private void Update()
    {
        if (time > 0) time -= Time.deltaTime;
        else PoolManager.Return(gameObject, GetType());
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            enemy.TakeDamage(2, false);
            enemy.OnBurn();
        }
    }
}