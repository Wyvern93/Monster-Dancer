using JetBrains.Annotations;
using System.Linq.Expressions;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.InputSystem.Android;

public class CarrotJuiceFlow : MonoBehaviour, IDespawneable
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] EdgeCollider2D edgeCollider;

    public float offset;
    public float beats;
    public float width;
    Vector2 diff;
    public float maxHeight = 2f;

    public Vector3 start, end;
    public float yoffset;
    public float frameCD = 0.1f;
    List<Enemy> enemies;
    float dmg;

    private void OnEnable()
    {
        enemies = new List<Enemy>();
        int level = (int)Player.instance.abilityValues["ability.carrotjuice.level"];
        dmg = level < 6 ? level < 4 ? level < 2 ? 4f : 6f : 8f : 12f;
        width = 0;
        beats = 0.5f;
    }

    public void OnDespawn()
    {
        beats = 0;
    }

    void Update()
    {
        if (beats > 0)
        {
            width = Mathf.MoveTowards(width, 1f, Time.deltaTime * 4f);
            if (BeatManager.isQuarterBeat) beats -= 0.25f;
            SetCollisions();
        }
        else width = Mathf.MoveTowards(width, 0, Time.deltaTime * 4f);

        if (frameCD > 0) frameCD -= Time.deltaTime;
        else
        {
            frameCD += 0.1f;
            if (yoffset == 0) yoffset = 0.5f;
            else yoffset = 0;
        }

        //time -= Time.deltaTime;
        offset -= Time.deltaTime * 5f;
        lineRenderer.widthMultiplier = width;
        lineRenderer.material.mainTextureOffset = new Vector2(offset, yoffset + 0.25f);

        UpdatePositions();

        if (BeatManager.isQuarterBeat)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                float damage = Player.instance.currentStats.Atk * dmg;
                if (damage < 1) damage = 1;
                bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
                if (isCritical) damage *= Player.instance.currentStats.CritDmg;
                if (enemies[i].CanBeSlowed(false)) enemies[i].OnSlow(1, 0.5f);
                enemies[i].TakeDamage((int)damage, isCritical);
            }
        }
        
        if (beats <= 0 && width <= 0)
        {
            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, GetType());
        }
    }


    private float CalculateHeight(float position)
    {
        if (position == 0) return 0;
        if (position == 1) return 0.25f;
        if (position == 2) return 0.35f;
        if (position == 3) return 0.4f;
        if (position == 4) return 0.35f;
        if (position == 5) return 0.25f;
        if (position == 6) return 0;
        
        float distanceToEnemy = 5 - position;
        float maxDistance = (start - end).magnitude;
        float result = Mathf.Sin((distanceToEnemy / maxDistance) * Mathf.PI) * maxHeight;
        maxHeight = maxDistance / 2f;
        result = Mathf.Clamp(result, 0, maxHeight);

        return result;
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

    private void UpdatePositions()
    {
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;

        if (difference == Vector2.zero) difference = Vector2.right;

        transform.position = Player.instance.Sprite.transform.position + ((Vector3)difference * 0.5f);

        start = Vector3.zero;
        end = (Vector3)crosshairPos - transform.localPosition;
        diff = end / 6f;

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(6, end);
        for (int i = 1; i < 6; i++)
        {
            Vector3 pos = (diff * i);
            lineRenderer.SetPosition(i, pos + new Vector3(0, CalculateHeight(i), CalculateHeight(i)));
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemies.Add(collision.GetComponent<Enemy>());
            Enemy enemy = collision.GetComponent<Enemy>();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (enemies.Contains(collision.GetComponent<Enemy>()))
            {
                enemies.Remove(collision.GetComponent<Enemy>());
            }
        }
    }

    public void ForceDespawn(bool instant = false)
    {
        if (instant) PoolManager.Return(gameObject, GetType());
        else
        {
            beats = 0;
        }
    }

}