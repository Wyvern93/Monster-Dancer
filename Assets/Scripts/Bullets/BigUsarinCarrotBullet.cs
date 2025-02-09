using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigUsarinCarrotBullet : Bullet
{
    public float origSpeed;
    public float beatTime;
    public float angle;
    public bool rotateRight;
    public int ai;

    public override void OnSpawn()
    {
        base.OnSpawn();
        angle = Vector2.SignedAngle(Vector2.down, direction);
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, angle);
        //OnBeat();
    }
    protected override IEnumerator MoveInDirection(Vector2 direction)
    {
        beatTime = 1;
        
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        speed = origSpeed;

        float beatDuration = BeatManager.GetBeatDuration();
        float fullRotationAmount = speed * (rotateRight ? -1 : 1) * beatDuration;
        float initialAngle = angle;

        while (time <= beatDuration)
        {
            // Calculate proportion of beat passed (0 to 1)
            float beatProgress = time / beatDuration;
            origSpeed -= Time.deltaTime * 6f;
            speed = origSpeed;
            
            // Calculate the current rotation based on the full rotation amount and beat progress
            float currentRotation = Mathf.Lerp(0, fullRotationAmount, beatProgress);
            angle = initialAngle + currentRotation;

            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Apply rotation to the sprite
            if (speed < 0)
            {
                spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(spriteRenderer.transform.localEulerAngles.z, angle+ 180, Time.deltaTime * 320f));
            }
            else
            {
                spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(spriteRenderer.transform.localEulerAngles.z, angle, Time.deltaTime * 320f));
            }
            

            // Calculate beatTime to achieve pulsing effect (affecting only movement)
            beatTime = Mathf.Lerp(1, 0f, beatProgress);

            // Move position based on direction, speed, and beatTime
            transform.position += (Vector3)direction * (speed * beatTime) * Time.deltaTime;

            // Increment time
            time += Time.deltaTime;
            yield return null;
        }
        beatTime = 0;
        yield break;
    }

    public override void Despawn()
    {
        base.Despawn();
    }

    public override void OnBeat()
    {
        lifetime--;
        if (superGrazed) Pulse();
        if (lifetime == 0)
        {
            spriteRenderer.color = Color.red;
            Despawn();
        }
        if (ai == 0)
        {
            StartCoroutine(MoveInDirection(direction));
        }
        else if (ai == 1)
        {
            StartCoroutine(MoveNormally(direction, true));
        }
        else if (ai == 2)
        {
            StartCoroutine(MoveNormally(direction));
        }
    }

    protected IEnumerator MoveNormally(Vector2 direction, bool rotate = false)
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
            speed = origSpeed;
            transform.position += ((Vector3)direction * speed * beatTime * Time.deltaTime);
            time += Time.deltaTime;
            if (rotate)
            {
                spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, spriteRenderer.transform.localEulerAngles.z + (360 * Time.deltaTime));
            }
            else
            {
                if (speed < 0)
                {
                    spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(spriteRenderer.transform.localEulerAngles.z, angle + 180, Time.deltaTime * 320f));
                }
                else
                {
                    spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(spriteRenderer.transform.localEulerAngles.z, angle, Time.deltaTime * 320f));
                }
            }
            

            yield return null;
        }

        yield break;
    }
}