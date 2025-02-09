using UnityEngine;

public class ConstellationLaser : MonoBehaviour
{
    public int atk = 4;
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (GameManager.isPaused) return;
        if (!BeatManager.isBeat) return;

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (GameManager.isPaused) return;

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }
}