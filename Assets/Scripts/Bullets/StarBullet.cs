using System.Collections;
using UnityEngine;

public class StarBullet : Bullet
{
    public float origSpeed;
    public override void OnSpawn()
    {
        base.OnSpawn();
    }
    protected override IEnumerator MoveInDirection(Vector2 direction)
    {
        float sprvelocity = direction.x > 0 || direction.y > 0 ? 250f : -250f;
        
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        speed = origSpeed;
        while (time <= BeatManager.GetBeatDuration())
        {
            speed = Mathf.Lerp(speed, 0, Time.deltaTime / BeatManager.GetBeatDuration() * 3);
            spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, spriteRenderer.transform.localEulerAngles.z + (sprvelocity * speed * Time.deltaTime));
            transform.position += ((Vector3)direction * speed * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
}