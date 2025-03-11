using UnityEngine;

public class HolyOrb : Drop
{
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, Time.deltaTime * speed);
        speed = Mathf.Clamp(speed + Time.deltaTime * 16f, 0, 64f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.Heal(1);
            PoolManager.Return(gameObject, GetType());
        }
    }
}