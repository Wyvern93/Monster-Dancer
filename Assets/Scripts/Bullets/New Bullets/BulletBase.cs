using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : Bullet
{
    public Animator animator;

    public float angle;
    protected float beatTime;
    public List<BulletBehaviour> behaviours;
    public override void OnSpawn()
    {
        if (Map.Instance != null) Map.Instance.bulletsSpawned.Add(this);

        angle = Vector2.SignedAngle(Vector2.down, direction) - 90;
        beat = 0;
        beatScale = 1;
        circleCollider.enabled = false;
        spriteRenderer.color = Color.clear;
        StartCoroutine(BulletSpawnCoroutine());
    }

    public override void OnBeat()
    {
        beat++;
        lifetime--;
        if (superGrazed) Pulse();
        if (lifetime == 0)
        {
            spriteRenderer.color = Color.red;
            Despawn();
        }
        StartCoroutine(BeatCoroutine());
    }

    public IEnumerator BeatCoroutine()
    {
        float beatDuration = BeatManager.GetBeatDuration();
        beatTime = 1;

        
        foreach (BulletBehaviour behaviour in behaviours)
        {
            behaviour.OnTrigger(this);
        }

        float time = 0;
        while (time <= beatDuration)
        {
            float beatProgress = time / beatDuration;
            beatTime = Mathf.Lerp(1, 0f, beatProgress);

            foreach(BulletBehaviour b in behaviours)
            {
                b.OnUpdate(this, beatTime);
            }

            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            transform.position += ((Vector3)direction * speed * beatTime * Time.deltaTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}