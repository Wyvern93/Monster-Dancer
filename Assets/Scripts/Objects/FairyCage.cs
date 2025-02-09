using System.Collections;
using UnityEngine;

public class FairyCage : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip breakSound, hitSound;
    [SerializeField] Fairy fairy;

    private int hits = 12;

    Vector2 position;

    public void OnEnable()
    {
        boxCollider.enabled = true;
        animator.Play("FairyCage_Idle");
        hits = 12;
        position = transform.position;
    }

    public void Break()
    {
        animator.Play("FairyCage_Break");
        AudioController.PlaySound(breakSound);
        boxCollider.enabled = false;
        fairy.OnCageBreak();
    }

    public void OnHit()
    {
        hits--;
        if (hits <= 0)
        {
            Break();
        }
        else
        {
            AudioController.PlaySound(hitSound);
            StartCoroutine(hitCoroutine());
        }
    }

    private IEnumerator hitCoroutine()
    {
        int shake = 12;
        while (shake > 0)
        {
            transform.position = position + ((Vector2)Random.insideUnitCircle * 0.03f);
            shake--;
            yield return null;
        }
        yield break;
    }
}