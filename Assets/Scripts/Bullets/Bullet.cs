using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] CircleCollider2D circleCollider;
    public Vector2 direction;
    public int beatsLeft;

    public bool superGrazed, grazed;
    private SpriteRenderer grazePulse;
    public Enemy enemySource;
    public int atk = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSuperGrazed()
    {
        if (superGrazed) return;
        AudioController.PlaySound(AudioController.instance.sounds.superGrazeSound);
        superGrazed = true;
        grazePulse = PoolManager.Get<SpriteRenderer>();
        grazePulse.sprite = spriteRenderer.sprite;
        grazePulse.transform.parent = spriteRenderer.transform;
        grazePulse.transform.localPosition = new Vector3(0, 0, 0.25f);
        grazePulse.transform.localEulerAngles = Vector3.zero;
        grazePulse.transform.localScale = Vector3.one;
        grazePulse.color = new Color(1, 1, 0, 0);
    }

    public void Pulse()
    {
        grazePulse.transform.localScale = Vector3.one;
        grazePulse.color = new Color(1, 1, 0, 1);

    }

    public void Graze()
    {
        grazed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!BeatManager.isPlaying) return;
        if (BeatManager.isGameBeat) OnBeat();

        if (!superGrazed) return;
        grazePulse.color = Color.Lerp(grazePulse.color, new Color(1, 1, 0, 0), Time.deltaTime * 16f);
        grazePulse.transform.localScale = Vector3.Lerp(grazePulse.transform.localScale, Vector3.one * 1.5f, Time.deltaTime * 16f);
    }

    public virtual void OnSpawn()
    {
        circleCollider.enabled = true;
        spriteRenderer.color = Color.white;
        beatsLeft = 6;
        OnBeat();
    }

    public void Despawn()
    {
        if (beatsLeft > 0)
        {
            beatsLeft = 0;
            StartCoroutine(DespawnCoroutine());
        }
    }

    protected void OnBeat()
    {
        beatsLeft--;
        if (superGrazed) Pulse();
        if (beatsLeft == 0)
        {
            spriteRenderer.color = Color.red;
            StartCoroutine(DespawnCoroutine());
        }
        StartCoroutine(MoveInDirection(direction));
    }

    IEnumerator DespawnCoroutine()
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

        if (enemySource != null) if (!enemySource.isActiveAndEnabled) SpawnDrop();

        if (superGrazed)
        {
            RabbitReflexesAbility reflexes = (RabbitReflexesAbility)Player.instance.equippedPassiveAbilities.FirstOrDefault(x => x.GetType() == typeof(RabbitReflexesAbility));
            if (reflexes != null)
            {
                reflexes.OnBulletDespawn(this);
            }
        }

        superGrazed = false;
        grazePulse = null;
        grazed = false;

        PoolManager.Return(gameObject, typeof(Bullet));
    }

    public void SpawnDrop()
    {
        BulletGem gem = PoolManager.Get<BulletGem>();
        gem.transform.position = (Vector2)transform.position + (Random.insideUnitCircle * 0.2f);
    }

    IEnumerator MoveInDirection(Vector2 direction)
    {
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, direction) - 90);
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
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (!BeatManager.isGameBeat) return;

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }
}
