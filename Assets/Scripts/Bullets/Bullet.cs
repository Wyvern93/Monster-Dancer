using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] protected CircleCollider2D circleCollider;
    [SerializeField] protected BoxCollider2D boxCollider;
    public bool boxHitbox;
    public Vector2 direction;
    public int lifetime;
    public float speed;

    public bool superGrazed, grazed;
    protected SpriteRenderer grazePulse;
    public Enemy enemySource;
    public int atk = 1;

    protected Material spriteRendererMat;
    private float emission = 0;
    protected float beatScale = 1;
    public int beat;
    public StunEffect stunStatus;
    public bool forcedDespawn;
    protected bool isInitialized;
    protected float beatCD;

    private void Awake()
    {
        stunStatus = new StunEffect();
        spriteRendererMat = spriteRenderer.material;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnStun(int time)
    {
        stunStatus.duration = time;
        spriteRenderer.color = new Color(0.8f, 0.8f, 1f, 1f);
    }

    public void OnStunEnd()
    {
        spriteRenderer.color = Color.white;
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
        if (GameManager.isPaused) return;

        if (beatCD <= 0)
        {
            beatCD += BeatManager.GetBeatDuration();
            if (!stunStatus.isStunned()) OnBeat();
            beatScale = 1.25f;
            if (stunStatus.duration > 0)
            {
                if (stunStatus.duration == 1) OnStunEnd();
                stunStatus.duration--;
            }
        }
        else
        {
            beatCD -= Time.deltaTime;
        }

        /*
        if (BeatManager.isBeat)
        {
            if (!stunStatus.isStunned()) OnBeat();

            beatScale = 1.25f;
        }

        if (BeatManager.isBeat && stunStatus.duration > 0)
        {
            if (stunStatus.duration == 1) OnStunEnd();
            stunStatus.duration--;
        }*/

        emission -= Time.deltaTime * 1f;
        if (emission < -0.1) emission = 0.03f;
        beatScale = Mathf.MoveTowards(beatScale, 1, Time.deltaTime * 2f);
        spriteRenderer.transform.localScale = Vector3.one * beatScale;
        spriteRendererMat.SetColor("_EmissionColor", new Color(1, 1, 1, emission));

        if (!stunStatus.isStunned()) OnBulletUpdate();

        if (!superGrazed) return;
    }

    public virtual void OnBulletUpdate() { }

    public void OnEnable()
    {
        if (Stage.Instance != null) SceneManager.MoveGameObjectToScene(gameObject, Stage.Instance.gameObject.scene);
    }
    public virtual void OnSpawn()
    {
        if (Stage.Instance != null) Stage.Instance.bulletsSpawned.Add(this);

        beat = 0;
        beatScale = 1;
        circleCollider.enabled = false;
        boxCollider.enabled = false;
        spriteRenderer.color = Color.clear;

        isInitialized = true;
        StartCoroutine(BulletSpawnCoroutine());
        if (BeatManager.isBeat) OnBeat();
        
        
    }

    protected IEnumerator BulletSpawnCoroutine()
    {
        float finalSize = transform.localScale.x;
        transform.localScale = Vector3.one * finalSize * 2f;
        spriteRenderer.color = new Color(1,0,0,0);

        while (transform.localScale.x > finalSize + 0.02f)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, Time.deltaTime * 16f);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * finalSize, Time.deltaTime * 8f);
            yield return null;
        }
        transform.localScale = Vector3.one * finalSize;
        spriteRenderer.color = Color.white;
        if (boxHitbox) boxCollider.enabled = true;
        else circleCollider.enabled = true;
        yield break;
    }

    public virtual void Despawn()
    {
        StopAllCoroutines();
        forcedDespawn = true;
        if (lifetime > 0)
        {
            lifetime = 0;
        }
        if (!gameObject.activeSelf) return;
        StartCoroutine(DespawnCoroutine(false));
    }

    public virtual void ForceDespawn()
    {
        StopAllCoroutines();
        lifetime = 0;
        if (!gameObject.activeSelf) return;
        StartCoroutine(DespawnCoroutine(true));
    }

    public virtual void OnBeat()
    {
        lifetime--;
        if (superGrazed) Pulse();
        if (lifetime == 0)
        {
            spriteRenderer.color = Color.red;
            Despawn();
            //StartCoroutine(DespawnCoroutine(false));
        }
        StartCoroutine(MoveInDirection(direction));
    }

    protected IEnumerator DespawnCoroutine(bool forced)
    {
        boxCollider.enabled = false;
        circleCollider.enabled = false;
        float time = 0;
        while (time <= BeatManager.GetBeatDuration())
        {
            spriteRenderer.transform.localScale = Vector3.Lerp(spriteRenderer.transform.localScale, Vector3.one * 1.5f, Time.deltaTime * 8f);
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, new Color(1, 0, 0, 0), Time.deltaTime * 8f);
            time += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.transform.localScale = Vector3.one;

        if (enemySource != null) if (!enemySource.isActiveAndEnabled) SpawnDrop();

        superGrazed = false;
        grazePulse = null;
        grazed = false;
        speed = 0;
        isInitialized = false;

        if (!forced && Stage.Instance != null) Stage.Instance.bulletsSpawned.Remove(this);
        transform.parent = null;
        PoolManager.Return(gameObject, GetType());
    }

    public void SpawnDrop()
    {
        //BulletGem gem = PoolManager.Get<BulletGem>();
        //gem.transform.position = (Vector2)transform.position + (Random.insideUnitCircle * 0.2f);
    }

    protected virtual IEnumerator MoveInDirection(Vector2 direction)
    {
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, direction) - 90);
        Vector3 originalPos = transform.position;
        Vector3 targetPos = (Vector3)direction + originalPos;
        float time = 0;
        
        while (time <= BeatManager.GetBeatDuration())
        {
            transform.position = Vector3.Lerp(transform.position, (Vector3)targetPos, time / 1.5f);
            time += Time.deltaTime;
            yield return null;
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

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.isPaused) return;
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (GameManager.isPaused) return;
        if (!BeatManager.isBeat) return;

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            Player.instance.TakeDamage(atk);
        }
    }
}
