using System.Collections;
using UnityEngine;

public class BlueUsarinCarrotBullet : Bullet
{
    public float origSpeed;
    float beatTime;

    public float angle;
    public float targetSpeed;
    public int ai;

    public Animator animator;
    public override void OnSpawn()
    {
        base.OnSpawn();
        angle = Vector2.SignedAngle(Vector2.down, direction) - 90;
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, angle);
        //OnBeat();
    }
    protected override IEnumerator MoveInDirection(Vector2 direction)
    {
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        
        beatTime = 1;

        float beatDuration = BeatManager.GetBeatDuration();

        while (time <= beatDuration)
        {
            float beatProgress = time / beatDuration;
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            beatTime = Mathf.Lerp(1, 0f, beatProgress);
            if (ai == 0)
            {
                origSpeed = Mathf.MoveTowards(origSpeed, targetSpeed, beatTime / 8);
                angle += Time.deltaTime * beatProgress * 70f;
            }
            
            speed = origSpeed;
            transform.position += ((Vector3)direction * speed * beatTime * Time.deltaTime);
            time += Time.deltaTime;
            if (speed < 0)
            {
                spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(spriteRenderer.transform.localEulerAngles.z, angle + 180, Time.deltaTime * 320f));
            }
            else
            {
                spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(spriteRenderer.transform.localEulerAngles.z, angle, Time.deltaTime * 320f));
            }

            yield return null;
        }

        yield break;
    }
}