using System.Collections.Generic;
using UnityEngine;

public class LunarAura : MonoBehaviour, IDespawneable
{
    [SerializeField] SpriteRenderer sprite;
    Color color = Color.white;
    public int beats;
    public float dmg;
    public int level;
    private Transform playerTransform;
    private List<Enemy> enemies = new List<Enemy>();
    private float cd;
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] AudioClip loseShieldSfx;

    public void OnLoseShield()
    {
        beats = 0;
        LunarPulse pulse = PoolManager.Get<LunarPulse>();
        pulse.transform.position = transform.position;
        pulse.dmg = dmg * 2f;
        AudioController.PlaySound(loseShieldSfx, side:true);
        PlayerCamera.TriggerCameraShake(0.3f, 0.5f);

        ForceDespawn();
    }

    public void OnEnable()
    {
        enemies.Clear();
        level = (int)Player.instance.abilityValues["ability.lunaraura.level"];
        beats = 8;
        sprite.color = color;
        dmg = level < 4 ? level < 2 ? 4f : 6f : 8f;
        playerTransform = Player.instance.transform;
        transform.localScale = Vector3.one * (level < 6 ? 1f : 1.25f);
        cd = 0;
        color.a = 0;
        circleCollider.enabled = false;
    }

    public void OnSpawn()
    {
        transform.position = playerTransform.position;
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - (Time.deltaTime * 100));
    }
    public void Update()
    {
        if (BeatManager.isGameBeat && level < 7)
        {
            beats--;
        }
        circleCollider.enabled = color.a >= 0.5f;
        if (beats > 0) color.a = Mathf.MoveTowards(color.a, 1f, Time.deltaTime * 2f);
        else color.a = Mathf.MoveTowards(color.a, 0f, Time.deltaTime * 2f);
        sprite.color = color;

        transform.position = playerTransform.position;

        if (cd <= 0)
        {
            cd = BeatManager.GetBeatDuration() / 4f;
            for (int i = 0; i < enemies.Count; i++)
            {
                float damage = (int)(Player.instance.currentStats.Atk * dmg);
                bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
                if (isCritical) damage *= Player.instance.currentStats.CritDmg;

                if (level >= 5)
                {
                    Vector2 dir = enemies[i].transform.position - Player.instance.transform.position;
                    enemies[i].PushEnemy(dir, 1f);
                }
                enemies[i].TakeDamage((int)damage, isCritical);
                foreach (PlayerItem item in Player.instance.equippedItems)
                {
                    item.OnHit(Player.instance.equippedPassiveAbilities.Find(x=> x.GetType() == typeof(LunarAuraAbility)), damage, enemies[i]);
                }
                foreach (PlayerItem item in Player.instance.evolvedItems)
                {
                    item.OnHit(Player.instance.equippedPassiveAbilities.Find(x => x.GetType() == typeof(LunarAuraAbility)), damage, enemies[i]);
                }
            }
        }
        else
        {
            cd -= Time.deltaTime;
        }

        if (color.a <= 0 && beats <= 0)
        {
            Player.instance.despawneables.Remove(this);
            beats = 4;
            PoolManager.Return(gameObject, typeof(LunarAura));
        }
    }

    public void ForceDespawn(bool instant = false)
    {
        if (instant) PoolManager.Return(gameObject, GetType());
        else OnDespawn();
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

    private void OnDespawn()
    {
        beats = 0;
    }
}