using UnityEngine;

public class AngelStatue : MonoBehaviour
{
    [SerializeField] Animator animator;
    bool used;

    private void OnEnable()
    {
        animator.speed = 1f / BeatManager.GetBeatDuration();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (Player.instance.CurrentHP >= Player.instance.currentStats.MaxHP) return;
        if (used) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            animator.Play("angelstatue_heal");
            Player.instance.Heal(Player.instance.currentStats.MaxHP * 0.2f, null);
            used = true;
        }
    }
}