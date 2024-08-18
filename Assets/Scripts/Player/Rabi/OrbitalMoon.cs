using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class OrbitalMoon : MonoBehaviour
{
    [SerializeField] SpriteTrail trail;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Sprite trailSprite;
    [SerializeField] CircleCollider2D circleCollider;

    public float angle;
    public int numBeats;
    private float blockChance;
    private float speedmulti = 1;
    private float dmg = 1;
    private int level;

    public void OnEnable()
    {
        level = (int)Player.instance.abilityValues["ability.orbitalmoon.level"];
        trail.Play(sprite, 35, 0.03f, transform, new Color(0, 1f, 1f, 0.5f), new Vector3(0,0, -0.5f));
        circleCollider.enabled = true;
        numBeats = level < 3 ? 6 : 12;
        sprite.color = new Color(1, 1, 1, 0);
        
        blockChance = level < 6 ? 10 : 25;
        speedmulti = level < 3 ? 1 : 1.25f;
        dmg = level < 4 ? level < 2 ? 20f : 24f : 31;
        transform.localScale = Vector3.one * (level < 6 ? 1f : 1.5f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = (int)(Player.instance.currentStats.Atk * dmg);
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)damage, isCritical);
            if (level >= 7)
            {
                Vector2 dir = enemy.transform.position - Player.instance.transform.position;
                enemy.PushEnemy(dir, 2f);
            }
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }

        if (collision.CompareTag("Bullet"))
        {
            bool destroyBullet = Random.Range(0f, 100f) <= blockChance;
            if (destroyBullet)
            {
                Bullet bullet = collision.GetComponent<Bullet>();
                bullet.Despawn();
            }
        }
    }

    public void Update()
    {
        angle += Time.deltaTime * 360f * speedmulti;
        Vector3 origin = Player.instance.transform.position;
        transform.position = new Vector3(origin.x + (2.5f * Mathf.Cos(angle * Mathf.Deg2Rad)), origin.y + (2.5f * Mathf.Sin(angle * Mathf.Deg2Rad)), origin.z + 2f);
        if (angle > 360) angle -= 360;

        if (numBeats > 1 && sprite.color.a < 1)
        {
            sprite.color = Color.Lerp(sprite.color, Color.white, Time.deltaTime * 8f);
        }

        if (BeatManager.isGameBeat && level < 7)
        {
            numBeats--;
            if (numBeats == 0)
            {
                Despawn();
            }
        }
    }

    public void Despawn()
    {
        StartCoroutine(OnDespawn());
    }

    private IEnumerator OnDespawn()
    {
        trail.Stop();
        while (sprite.color.a > 0.02f)
        {
            sprite.color = Color.Lerp(sprite.color, new Color(1, 1, 1, 0), Time.deltaTime * 2f);
            yield return new WaitForEndOfFrame();
        }
        PoolManager.Return(gameObject, GetType());
    }

}