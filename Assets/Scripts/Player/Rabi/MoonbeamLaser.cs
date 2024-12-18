using System.Collections.Generic;
using UnityEngine;

public class MoonbeamLaser : MonoBehaviour
{
    [SerializeField] private EdgeCollider2D edgeCollider;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] AudioClip sound;
    [SerializeField] Animator animator;

    public MoonBeamAbility abilitySource;

    public Vector3 start, end;
    public float abilityDamage;
    Vector2 diff;

    void OnEnable()
    {
        AudioController.PlaySound(sound);
        // Assign the sprite sheet texture to the LineRenderer
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    public void Init(float duration, float size)
    {
        animator.speed = (1f / BeatManager.GetBeatDuration()) / duration;
        edgeCollider.edgeRadius = size * 0.6f;
        lineRenderer.widthMultiplier = size * 2;
        lineRenderer.textureScale = new Vector2(0.5f / size, 1);
        SetPosition();
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

    public void OnFinish()
    {
        PoolManager.Return(gameObject, typeof(MoonbeamLaser));
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = (int)(abilityDamage);
            bool isCritical = abilitySource.GetCritChance() > Random.Range(0f, 100f);
            if (isCritical) damage *= 2.5f;
            enemy.TakeDamage((int)damage, isCritical);
            foreach (PlayerItem item in abilitySource.equippedItems)
            {
                if (item == null) continue;
                item.OnHit(abilitySource, damage, enemy, isCritical);
            }
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}