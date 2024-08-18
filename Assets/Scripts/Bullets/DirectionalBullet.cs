using System.Collections;
using UnityEngine;

public class DirectionalBullet : Bullet
{
    public float origSpeed;
    public override void OnSpawn()
    {
        base.OnSpawn();
    }
    protected override IEnumerator MoveInDirection(Vector2 direction)
    {
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, direction) - 90);
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        speed = origSpeed;
        while (time <= BeatManager.GetBeatDuration())
        {
            speed = Mathf.Lerp(speed, 0, Time.deltaTime / BeatManager.GetBeatDuration() * 4);
            transform.position += ((Vector3)direction * speed * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, (Vector3)targetPos, time / 1.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}