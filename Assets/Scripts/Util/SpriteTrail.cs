using System.Collections.Generic;
using UnityEngine;

public class SpriteTrail : MonoBehaviour
{
    public float perSpriteTime;
    public int numOfSprites;

    private float currentOffset;
    private Transform targetObj;
    private Vector3 sprOffset;
    private float size;

    private bool playing;

    List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    SpriteRenderer spriteReference;
    Color spriteColor;
    public void Play(SpriteRenderer reference, int numSprites, float offset, Transform obj, Color color, Vector3 spriteOffset, float size = 1)
    {
        ClearSprites();
        spriteColor = color;
        spriteReference = reference;
        perSpriteTime = offset;
        numOfSprites = numSprites;
        currentOffset = 0;
        targetObj = obj;
        playing = true;
        this.size = size;
        sprOffset = spriteOffset;
    }

    public void Stop()
    {
        playing = false;
    }

    private void ClearSprites()
    {
        foreach (SpriteRenderer s in sprites)
        {
            PoolManager.Return(s.gameObject, typeof(SpriteRenderer));
        }
        sprites.Clear();
    }

    public void Update()
    {
        if (GameManager.isPaused) return;
        if (spriteReference == null) return;
        if (!spriteReference.gameObject.activeSelf) return;

        if (currentOffset <= 0)
        {
            currentOffset = perSpriteTime;
            if (sprites.Count < numOfSprites)
            {
                Spawn();
            }
            else
            {
                DespawnFirst();
                Spawn();
            }
        }
        else
        {
            currentOffset -= Time.deltaTime;
        }

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = new Color(1, 1, 1, sprite.color.a - Time.deltaTime * 2f);
        }
    }

    public void Spawn()
    {
        if (!playing) return;
        SpriteRenderer spr = PoolManager.Get<SpriteRenderer>();
        spr.sprite = spriteReference.sprite;
        spr.sortingOrder = 2;
        spr.transform.position = targetObj.position + sprOffset;
        spr.color = new Color(1, 1, 1, 0.3f);
        spr.material.SetColor("_EmissionColor", spriteColor);
        spr.transform.localScale = targetObj.transform.localScale * size;
        spr.transform.eulerAngles = targetObj.eulerAngles;
        sprites.Add(spr);
    }

    public void DespawnFirst()
    {
        PoolManager.Return(sprites[0].gameObject, typeof(SpriteRenderer));
        sprites.RemoveAt(0);
    }

    public void ForceDespawn()
    {
        foreach(SpriteRenderer s in sprites)
        {
            PoolManager.Return(s.gameObject, typeof(SpriteRenderer));
        }
    }
}