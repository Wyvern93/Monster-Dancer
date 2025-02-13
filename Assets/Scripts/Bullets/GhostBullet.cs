using System.Collections;
using UnityEngine;

public class GhostBullet : Bullet
{
    public float origSpeed;
    public float angle;
    public bool canMove;
    public override void OnSpawn()
    {
        base.OnSpawn();
        angle = 0;
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, direction) - 90);
    }

    public override void OnBeat()
    {
        lifetime--;
        if (superGrazed) Pulse();
        if (lifetime == 0)
        {
            spriteRenderer.color = Color.red;
            StartCoroutine(DespawnCoroutine(false));
        }
        if (canMove) StartCoroutine(MoveInDirection(direction));
    }
    protected override IEnumerator MoveInDirection(Vector2 direction)
    {
        
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        speed = origSpeed;
        while (time <= BeatManager.GetBeatDuration())
        {
            speed = Mathf.Lerp(speed, 0, Time.deltaTime / BeatManager.GetBeatDuration() * 4);
            angle += (Time.deltaTime / BeatManager.GetBeatDuration()) * 180f;
            Vector2 angleDir = direction.normalized + (Vector2)((transform.up * Mathf.Sin(angle * Mathf.Deg2Rad))).normalized;
            

            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            //transform.position += ((Vector3)direction * speed * Time.deltaTime);
            //transform.position += (((Vector3)perpendicular * Mathf.Sin(angle * Mathf.Deg2Rad)));

            float waveOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * 0.5f;
            Vector2 waveDir = direction + perpendicular * waveOffset;
            spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, waveDir) - 90);
            transform.position += (Vector3)waveDir.normalized * speed * Time.deltaTime;
            //transform.position = Vector3.Lerp(transform.position, (Vector3)targetPos, time / 1.5f);
            time += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
}