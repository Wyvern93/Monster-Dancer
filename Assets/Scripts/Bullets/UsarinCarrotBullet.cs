using System.Collections;
using UnityEngine;

public class UsarinCarrotBullet : Bullet
{
    public float origSpeed;
    public float speedChange;
    float beatTime;
    public int phase = 0;

    public float angle;
    public override void OnSpawn()
    {
        if (Stage.Instance != null) Stage.Instance.bulletsSpawned.Add(this);

        beatScale = 1;
        circleCollider.enabled = false;
        boxCollider.enabled = false;
        spriteRenderer.color = Color.clear;
        StartCoroutine(BulletSpawnCoroutine());

        speedChange = 1;
        phase = 0;
        angle = Vector2.SignedAngle(Vector2.down, direction) - 90;
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
    protected override IEnumerator MoveInDirection(Vector2 direction)
    {
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
                speedChange = Mathf.MoveTowards(speedChange, 0, Time.deltaTime / beatDuration);
            }
            else if (phase > 3)
            {
                speedChange += Time.deltaTime * 2;
                Mathf.Clamp(speedChange, 0, 3);
            }
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            beatTime = Mathf.Lerp(1, 0f, beatProgress);

            transform.position += ((Vector3)direction * speed * beatTime * speedChange * Time.deltaTime);
            time += Time.deltaTime;
            spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(spriteRenderer.transform.localEulerAngles.z, angle, Time.deltaTime * 320f));

            yield return new WaitForEndOfFrame();
        }
        if (speedChange <= 0)
        {
            phase++;
        }

        yield break;
    }
}