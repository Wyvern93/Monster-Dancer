using UnityEngine;

public class LightAttack : MonoBehaviour
{
    [SerializeField] private AudioClip lightAttackSound;
    public int atk = 4;
    public bool playSound;

    public void OnEnable()
    {
        GetComponent<Animator>().speed = 1f / BeatManager.GetBeatDuration();
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.isPaused) return;
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }

    public void OnAnimationEnd()
    {
        PoolManager.Return(gameObject, typeof(LightAttack));
    }

    public void OnPlaySound()
    {
        if (!playSound) return;
        AudioController.PlaySound(lightAttackSound);
    }
}