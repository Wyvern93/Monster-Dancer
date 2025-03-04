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
    public bool startOnBeat;

    public override void OnSpawn()
    {
        StopAllCoroutines();
        beatTime = 0;
        forcedDespawn = false;
        animator.speed = 1f / BeatManager.GetBeatDuration();
        if (Stage.Instance != null) Stage.Instance.bulletsSpawned.Add(this);

        angle = Vector2.SignedAngle(Vector2.down, direction) - 90;
        beat = 0;
        beatScale = 1;
        circleCollider.enabled = false;
        boxCollider.enabled = false;
        spriteRenderer.color = Color.clear;
        isInitialized = true;
        StartCoroutine(BulletSpawnCoroutine());
        //if (startOnBeat) OnBeat();
        beatCD = BeatManager.GetBeatDuration();
        foreach (BulletBehaviour behaviour in behaviours)
        {
            behaviour.OnSpawn(this);
        }
        OnBeat();
    }

    public static Vector2 angleToVector(float degrees)
    {
        return new Vector2(Mathf.Cos(degrees * Mathf.Deg2Rad), Mathf.Sin(degrees * Mathf.Deg2Rad)).normalized;
    }

    public static float VectorToAngle(Vector2 vector)
    {
        return Vector2.SignedAngle(Vector2.down, vector) - 90;
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
        if (!isInitialized) yield break;
        float beatDuration = BeatManager.GetBeatDuration();
        beatTime = 1;

        foreach (BulletBehaviour behaviour in behaviours)
        {
            behaviour.OnTrigger(this);
        }
        float time = 0;
        while (time <= beatDuration)
        {
            if (!isInitialized) yield break;
            while (GameManager.isPaused || stunStatus.isStunned())
            {
                yield return null;
            }
            float beatProgress = time / beatDuration;
            beatTime = Mathf.Lerp(1, 0f, beatProgress);

            foreach(BulletBehaviour b in behaviours)
            {
                b.OnUpdate(this, beatTime);
            }

            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            transform.position += ((Vector3)direction * speed * Time.deltaTime * 0.5f);//beatTime * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        yield break;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.isPaused) return;
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            foreach (BulletBehaviour b in behaviours)
            {
                b.OnPlayerHit(this);
            }
            if (atk > 0) Player.instance.TakeDamage(atk);

        }
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (GameManager.isPaused) return;
        if (!BeatManager.isBeat) return;

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            foreach (BulletBehaviour b in behaviours)
            {
                b.OnPlayerHit(this);
            }
            if (atk > 0) Player.instance.TakeDamage(atk);
        }
    }

    public override void Despawn()
    {
        foreach (BulletBehaviour behaviour in behaviours)
        {
            behaviour.OnDespawn(this);
        }
        base.Despawn();
    }
}