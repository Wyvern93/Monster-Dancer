using System.Collections;
using UnityEngine;

public class SpiralBullet : Bullet
{
    public float origSpeed;
    float distance = 1;
    public float orbitAngle;
    public float orbitSpeed;
    Vector3 origin;
    public Vector3 directionalVelocity;
    public override void OnSpawn()
    {
        base.OnSpawn();
        distance = 0.5f;
        origin = enemySource.transform.position;
    }

    public override void OnBulletUpdate()
    {
        base.OnBulletUpdate();
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
        if (enemySource != null) StartCoroutine(MoveCoroutine());

    }

    IEnumerator MoveCoroutine()
    {
        float time = 0;
        speed = origSpeed;
        while (time <= BeatManager.GetBeatDuration())
        {
            if (enemySource == null)
            {
                Despawn();
                yield break;
            }
            speed = Mathf.Lerp(speed, 0, Time.deltaTime / BeatManager.GetBeatDuration() * 4);
            origin += (directionalVelocity * speed * Time.deltaTime);
            orbitAngle += speed * 480 * Time.deltaTime;
            distance += speed * orbitSpeed * Time.deltaTime;
            transform.position = new Vector3(origin.x + (Mathf.Cos(orbitAngle * Mathf.Deg2Rad) * distance), origin.y + 0.3f + (Mathf.Sin(orbitAngle * Mathf.Deg2Rad) * distance), 0);
            Debug.Log(directionalVelocity);
            Debug.Log(speed);
            
            time += Time.deltaTime;
            yield return null;
        }

        yield break;
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
            yield return null;
        }

        yield break;
    }
}