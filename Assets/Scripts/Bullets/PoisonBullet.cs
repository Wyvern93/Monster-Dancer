using System.Collections;
using UnityEngine;

public class PoisonBullet : Bullet
{
    public float origSpeed;
    public float currentSpeed;
    float dmgcd;
    public override void OnSpawn()
    {
        base.OnSpawn();
        currentSpeed = 4;
        transform.localScale = Vector3.one;
    }
    protected override IEnumerator MoveInDirection(Vector2 direction)
    {
        //spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, direction) - 90);
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        speed = currentSpeed;
        while (time <= BeatManager.GetBeatDuration())
        {
            speed = Mathf.Lerp(speed, 0, Time.deltaTime / BeatManager.GetBeatDuration() * 4);
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.2f, Time.deltaTime / BeatManager.GetBeatDuration() / 3f);
            transform.position += ((Vector3)direction * speed * currentSpeed * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, (Vector3)targetPos, time / 1.5f);
            time += Time.deltaTime;
            transform.localScale *= (1 + (1 * speed * 0.5f) * Time.deltaTime);
            yield return null;
        }

        yield break;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (dmgcd > 0)
        {
            dmgcd -= Time.deltaTime;
            return;
        }
        else
        {
            dmgcd = 0.2f;
        }

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }
}