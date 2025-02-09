using System.Collections;
using UnityEngine;

public class CutsceneActor : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] string idleAnim;
    [SerializeField] string moveAnim;
    [SerializeField] string jumpAnim;
    
    [Header("Data")]
    public float speed = 1.5f;
    public Animator animator;
    public SpriteRenderer sprite;
    public SpriteRenderer shadow;

    public bool facingRight = true;
    float beatTime = 1;

    public bool isActionFinished;

    private void Update()
    {
        sprite.flipX = !facingRight;
    }

    public void MoveTo(Vector2 target)
    {
        isActionFinished = false;
        StartCoroutine(MoveCoroutine(target));
    }

    public void JumpTo(Vector2 target) 
    {
        isActionFinished = false;
        StartCoroutine(JumpCoroutine(target));
    }

    public void TeleportTo(Vector2 target) 
    {
        transform.position = target;
        isActionFinished = true;
    }

    public void PlayAnimation(string animation, bool fullBeat, bool beatAnimation = true)
    {
        animator.Play(animation);
        if (beatAnimation)
        {
            if (fullBeat) animator.speed = 1 / BeatManager.GetBeatDuration();
            else animator.speed = 1 / BeatManager.GetBeatDuration() * 2f;
        }
        else
        {
            animator.speed = 1;
        }

        isActionFinished = true;
    }

    protected IEnumerator MoveCoroutine(Vector2 target)
    {
        Debug.Log("Asked to move");
        while ((Vector2)transform.position != target)
        {
            while (!BeatManager.isBeat) yield return null;

            animator.Play(moveAnim);
            animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
            float duration = BeatManager.GetBeatDuration() * 0.5f;
            float time = 0;
            facingRight = target.x > transform.position.x;

            while (time <= duration)
            {
                while (GameManager.isPaused) yield return null;
                time += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed * 6);
                yield return null;
            }

            animator.speed = 1f / BeatManager.GetBeatDuration();
            animator.Play(idleAnim);
            yield return null;
            sprite.transform.localPosition = Vector3.zero;
        }
        isActionFinished = true;
    }

    protected IEnumerator JumpCoroutine(Vector2 target)
    {
        while ((Vector2)transform.position != target)
        {
            while (!BeatManager.isBeat) yield return null;

            animator.Play(jumpAnim);
            animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
            float duration = BeatManager.GetBeatDuration() * 0.5f;
            float time = 0;
            facingRight = target.x > transform.position.x;
            beatTime = 1;

            while (time <= duration)
            {
                while (GameManager.isPaused) yield return null;

                float beatProgress = time / duration;
                beatTime = Mathf.Lerp(1, 0f, beatProgress);

                time += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, target, beatTime);
                yield return null;
            }

            animator.speed = 1f / BeatManager.GetBeatDuration();
            animator.Play(idleAnim);
            yield return null;
            sprite.transform.localPosition = Vector3.zero;
        }
        isActionFinished = true;
    }
}