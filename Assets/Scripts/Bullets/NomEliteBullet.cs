using System.Collections;
using UnityEngine;

public class NomEliteBullet : Bullet
{
    public float origSpeed;
    bool orbiting = true;
    float distance = 1;
    float orbitAngle;
    public override void OnSpawn()
    {
        base.OnSpawn();
        orbiting = true;
        orbitAngle = Vector2.SignedAngle(Vector2.down, direction) - 90;
        distance = 1;
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
        if (enemySource != null) StartCoroutine(OrbitCoroutine());

    }

    IEnumerator OrbitCoroutine()
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
            orbitAngle += speed * 50 * Time.deltaTime;
            distance += speed * 1f * Time.deltaTime;
            transform.position = new Vector3(enemySource.transform.position.x + (Mathf.Cos(orbitAngle * Mathf.Deg2Rad) * distance), enemySource.transform.position.y + 0.3f + (Mathf.Sin(orbitAngle * Mathf.Deg2Rad) * distance), 0);
            //transform.position = Vector3.Lerp(transform.position, (Vector3)targetPos, time / 1.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
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
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}