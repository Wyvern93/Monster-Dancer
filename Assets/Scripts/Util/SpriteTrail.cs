using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteTrail : MonoBehaviour
{
    public float perSpriteTime;
    public int numOfSprites;

    private float currentOffset;
    private Transform targetObj;

    private bool playing;

    List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    SpriteRenderer spriteReference;
    Color spriteColor;
    public void Play(SpriteRenderer reference, int numSprites, float offset, Transform obj, Color color)
    {
        ClearSprites();
        spriteColor = color;
        spriteReference = reference;
        perSpriteTime = offset;
        numOfSprites = numSprites;
        currentOffset = 0;
        targetObj = obj;
        playing = true;
    }

    public void Stop()
    {
        playing = false;
    }

    private void ClearSprites()
    {
        foreach (SpriteRenderer sprite in sprites)
        {
            GameObject.Destroy(sprite.gameObject);
        }
        sprites.Clear();
    }

    public void Update()
    {
        if (spriteReference == null) return;

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
        spr.transform.position = targetObj.position + new Vector3(0,0, -0.5f);
        spr.color = new Color(1, 1, 1, 0.3f);
        spr.material.SetColor("_EmissionColor", spriteColor);
        sprites.Add(spr);
    }

    public void DespawnFirst()
    {
        PoolManager.Return(sprites[0].gameObject, typeof(SpriteRenderer));
        sprites.RemoveAt(0);
    }
}