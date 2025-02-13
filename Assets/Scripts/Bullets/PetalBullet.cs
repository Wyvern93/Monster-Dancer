using System.Collections;
using UnityEngine;

public class PetalBullet : Bullet
{
    public float origSpeed;
    public bool changed;
    public bool follow;
    float currentSpeed;
    public override void OnSpawn()
    {
        base.OnSpawn();
        changed = false;
        currentSpeed = 3;
        OnBeat();
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
        
        //if (changed) StartCoroutine(MoveInDirection(direction));
        //else StartCoroutine(SlowlyMove(direction));
        if (!changed) StartCoroutine(SlowlyMove(direction));
        else StartCoroutine(MoveInDirection(direction));

    }

    protected override IEnumerator MoveInDirection(Vector2 direction)
    {
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, direction) - 90);
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        speed = origSpeed;
        Debug.Log(origSpeed);
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

    protected IEnumerator SlowlyMove(Vector2 direction)
    {
        
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        speed = origSpeed;
        while (time <= BeatManager.GetBeatDuration())
        {
            
            speed = Mathf.Lerp(speed, 0, Time.deltaTime / BeatManager.GetBeatDuration() * 4);
            
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, Time.deltaTime * 1.5f);
            spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, spriteRenderer.transform.localEulerAngles.z + (400 * speed * currentSpeed * Time.deltaTime));
            transform.position += ((Vector3)direction * speed * currentSpeed * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, (Vector3)targetPos, time / 1.5f);
            time += Time.deltaTime;

            yield return null;
        }
        if (!follow)
        {
            if (currentSpeed <= 0.05f)
            {
                Despawn();
            }
            yield break;
        }
            
        if (currentSpeed <= 0.05)
        {
            changed = true;
            Vector2 dir = Player.instance.GetClosestPlayer(transform.position) - transform.position;
            dir.Normalize();
            this.direction = dir;
            speed = 25;
            origSpeed = 25;
        }
        yield break;
    }
}