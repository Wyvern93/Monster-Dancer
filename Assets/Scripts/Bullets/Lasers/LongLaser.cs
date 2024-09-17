using UnityEngine;

public class LongLaser : MonoBehaviour
{
    public Animator animator;
    public int atk;
    [SerializeField] AudioSource sfx;
    [SerializeField] BoxCollider2D bc;
    public void OnEnable()
    {
        animator.speed = 1f / BeatManager.GetBeatDuration();
        bc.enabled = false;
        
    }

    public void OnLaserStart()
    {
        sfx.Play();
        bc.enabled = true;
    }

    public void OnDespawnAnimation()
    {
        bc.enabled = false;
        PoolManager.Return(gameObject, typeof(LongLaser));
    }

    public void Despawn()
    {
        bc.enabled = false;
        sfx.Stop();
        animator.Play("laser_despawn");
    }

    public void ForceDespawn()
    {
        sfx.Stop();
        PoolManager.Return(gameObject, typeof(LongLaser));
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

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        bc.enabled = false;
        if (!BeatManager.isPlaying) return;
        if (GameManager.isPaused) return;
        if (!BeatManager.isGameBeat) return;

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }

    public void Update()
    {
        transform.localScale = Vector3.one * Random.Range(0.9f, 1.12f);
    }
}