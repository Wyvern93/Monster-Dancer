using System.Collections.Generic;
using UnityEngine;

public class MoonbeamLaser : MonoBehaviour
{
    [SerializeField] private EdgeCollider2D edgeCollider;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Texture2D spriteSheet;
    [SerializeField] private int columns = 2; // Number of frames horizontally
    [SerializeField] private int rows = 2;    // Number of frames vertically
    [SerializeField] private float frameDuration = 0.1f; // Duration of each frame

    [SerializeField] private int currentFrame;
    [SerializeField] AudioClip sound;
    private float timer;
    private int frames;

    public Vector3 start, end;
    int level;
    float abilityDamage;
    Vector2 diff;

    void OnEnable()
    {
        int abilityLevel = (int)Player.instance.abilityValues["ability.moonbeam.level"];
        level = abilityLevel;

        abilityDamage = abilityLevel < 4 ? abilityLevel < 2 ? 40 : 60 : 80;
        AudioController.PlaySound(sound);
        SetPosition();
        timer = 0;
        frames = columns * rows;
        // Assign the sprite sheet texture to the LineRenderer
        lineRenderer.material.mainTexture = spriteSheet;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        UpdateTextureScale();
    }

    public void SetPosition()
    {
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;

        if (difference == Vector2.zero) difference = Vector2.right;

        transform.position = Player.instance.Sprite.transform.position + ((Vector3)difference) + new Vector3(0,0,3);

        start = Vector3.zero;
        end = difference * 12;
        diff = end / 4f;

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(5, end);
        for (int i = 1; i < 3; i++)
        {
            lineRenderer.SetPosition(i, (diff * i) + (Random.insideUnitCircle * 0.5f));
        }
        
    }

    private void Update()
    {
        for (int i = 1; i < 5; i++)
        {
            lineRenderer.SetPosition(i, (diff * i) + (Random.insideUnitCircle * 0.5f));
        }

        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;

        if (difference == Vector2.zero) difference = Vector2.right;

        transform.position = Player.instance.Sprite.transform.position + ((Vector3)difference * 0.2f);

        if (frames >= 0) edgeCollider.enabled = true;
        else edgeCollider.enabled = false;
        SetCollisions();
    }

    private void SetCollisions()
    {
        List<Vector2> edges = new List<Vector2>();
        for (int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 linePoint = lineRenderer.GetPosition(point);
            edges.Add(linePoint);
        }

        edgeCollider.SetPoints(edges);
    }

    void LateUpdate()
    {
        UpdateTextureScale();
        // Update the frame timer
        timer += Time.deltaTime;
        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame = (currentFrame + 1) % (columns * rows);
            frames--;
            if (frames >= 0) UpdateTextureOffset();
            else
            {
                lineRenderer.startColor = Color.clear;
                lineRenderer.endColor = Color.clear;
            }

        }
        if (frames <= -2)
        {
            timer = 0;
            frames = columns * rows;
            PoolManager.Return(gameObject, typeof(MoonbeamLaser));
        }
    }

    void UpdateTextureScale()
    {
        // Calculate scale based on the number of frames
        Vector2 scale = new Vector2(1f / columns, 1f / rows);
        lineRenderer.material.mainTextureScale = scale;
    }

    void UpdateTextureOffset()
    {
        // Calculate the offset for the current frame
        int column = currentFrame % columns;
        int row = currentFrame / columns;
        Vector2 offset = new Vector2(column / (float)columns, 1f - (row + 1) / (float)rows);

        lineRenderer.material.mainTextureOffset = offset;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = (int)(Player.instance.currentStats.Atk * abilityDamage);
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;
            enemy.TakeDamage((int)damage, isCritical);
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}