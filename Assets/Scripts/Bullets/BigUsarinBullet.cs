using System.Collections;
using UnityEngine;

public class BigUsarinBullet : Bullet
{
    public float origSpeed;
    public bool isPhase2;
    public float speedChange;
    float beatTime;
    int phase = 0;

    private float angle;
    public override void OnSpawn()
    {
        base.OnSpawn();
        speedChange = 1;
        phase = 0;
        angle = Vector2.SignedAngle(Vector2.down, direction);
        OnBeat();
    }
    protected override IEnumerator MoveInDirection(Vector2 direction)
    {

        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, direction) - 90);
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        speed = origSpeed;
        beatTime = 1;
        
        float beatDuration = BeatManager.GetBeatDuration();

        while (time <= beatDuration)
        {
            float beatProgress = time / beatDuration;
            if (phase == 0)
            {
                speedChange = Mathf.MoveTowards(speedChange, 0, Time.deltaTime / beatDuration / 2);
                //angle += 90 * Time.deltaTime;
            }
            else if (phase > 1)
            {
                speedChange += Time.deltaTime * 2;
                Mathf.Clamp(speedChange, 0, 3);
            }
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            beatTime = Mathf.Lerp(1, 0f, beatProgress);

            transform.position += ((Vector3)direction * speed * beatTime * speedChange * Time.deltaTime);
            time += Time.deltaTime;

            if (speedChange <= 0)
            {
                phase++;
            }
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}