using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Bullet : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] CircleCollider2D circleCollider;
    public Vector2 direction;
    public int beatsLeft;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BeatManager.isGameBeat) OnBeat();
    }

    public void OnSpawn()
    {
        circleCollider.enabled = true;
        spriteRenderer.color = Color.white;
        beatsLeft = 4;
        OnBeat();
    }

    protected void OnBeat()
    {
        beatsLeft--;
        if (beatsLeft == 1) spriteRenderer.color = Color.red;
        if (beatsLeft == 0)
        {
            StartCoroutine(Despawn());
        }
        StartCoroutine(MoveInDirection(direction));
    }

    IEnumerator Despawn()
    {
        circleCollider.enabled = false;
        float time = 0;
        while (time <= BeatManager.GetBeatDuration() / 2f)
        {
            spriteRenderer.transform.localScale = Vector3.Lerp(spriteRenderer.transform.localScale, Vector3.one * 1.5f, Time.deltaTime * 8f);
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, new Color(1, 0, 0, 0), Time.deltaTime * 8f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        spriteRenderer.transform.localScale = Vector3.one;

        BulletGem gem = PoolManager.Get<BulletGem>();
        gem.transform.position = transform.position;

        PoolManager.Return(gameObject, typeof(Bullet));
    }

    IEnumerator MoveInDirection(Vector2 direction)
    {
        Vector2 normalized = new Vector2(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, getAngleFromVector(normalized));
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            transform.position = Vector3.Lerp(transform.position, (Vector3)targetPos, time / 1.5f);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPos;
        yield break;
    }
    public float getAngleFromVector(Vector2 direction)
    {
        // Vertical
        if (direction.x == 0)
        {
            if (direction.y == 1) return 90f;
            if (direction.y == -1) return 270f;
        }

        // Horizontal
        if (direction.y == 0)
        {
            if (direction.x == 1) return 0f;
            if (direction.x == -1) return 180f;
        }

        // Diagonal Right
        if (direction.x == 1)
        {
            if (direction.y == 1) return 45f;
            if (direction.y == -1) return 315;
        }

        if (direction.x == -1)
        {
            if (direction.y == 1) return 135f;
            if (direction.y == -1) return 225f;
        }

        return 0;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(1);
        }

        if  (collision.CompareTag("Player") && collision.name == "GrazeTrigger")
        {
            AudioController.PlaySound(Map.Instance.grazeSound);
            Player.AddSP(5);
        }
    }
}
