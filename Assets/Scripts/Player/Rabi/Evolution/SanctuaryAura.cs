using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SanctuaryAura : MonoBehaviour, IDespawneable
{
    [SerializeField] SpriteRenderer sprite;
    Color color = Color.white;
    public int beats, holybeats;
    public float dmg;
    private Transform playerTransform;
    private List<Enemy> enemies = new List<Enemy>();
    private float cd;
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] AudioClip loseShieldSfx;

    [SerializeField] SpriteRenderer cross;
    [SerializeField] SanctuaryWave wave;
    public PlayerAbility abilitySource;

    public void OnLoseShield()
    {
        beats = 0;
        holybeats = 8;
        SanctuaryPulse pulse = PoolManager.Get<SanctuaryPulse>();
        pulse.transform.position = transform.position;
        pulse.dmg = dmg * 2f;
        AudioController.PlaySound(loseShieldSfx, side:true);
        PlayerCamera.TriggerCameraShake(0.3f, 0.5f);

        ForceDespawn();
    }

    public void OnEnable()
    {
        enemies.Clear();
        beats = 8;
        holybeats = 8;
        sprite.color = color;
        dmg = 8f;
        playerTransform = Player.instance.transform;
        transform.localScale = Vector3.one * 1.25f;
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
        circleCollider.enabled = color.a >= 0.5f;
        if (beats > 0) color.a = Mathf.MoveTowards(color.a, 1f, Time.deltaTime * 2f);
        else color.a = Mathf.MoveTowards(color.a, 0f, Time.deltaTime * 2f);
        sprite.color = color;
        cross.color = color;

        if (!GameManager.isPaused && BeatManager.isGameBeat)
        {
            holybeats--;
            if (holybeats == 0)
            {
                CastWave();
                holybeats = 8;
            }
        }

        transform.position = playerTransform.position;

        if (cd <= 0)
        {
            cd = BeatManager.GetBeatDuration() / 4f;
            for (int i = 0; i < enemies.Count; i++)
            {
                float damage = (int)(Player.instance.currentStats.Atk * dmg);
                bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
                if (isCritical) damage *= Player.instance.currentStats.CritDmg;

                Vector2 dir = enemies[i].transform.position - Player.instance.transform.position;
                enemies[i].PushEnemy(dir, 1f);

                if ((enemies[i].CurrentHP * 100f / enemies[i].MaxHP) <= 25f && enemies[i] is not Boss) damage = enemies[i].CurrentHP; // Execute
                enemies[i].TakeDamage((int)damage, isCritical);
                foreach (PlayerItem item in abilitySource.equippedItems)
                {
                    if (item == null) continue;
                    item.OnHit(abilitySource, damage, enemies[i]);
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
            beats = 2;
            Player.instance.equippedPassiveAbilities.FirstOrDefault(x => x.GetType() == typeof(SanctuaryAbilityEvolution)).currentCooldown = 4;
            PoolManager.Return(gameObject, typeof(SanctuaryAura));
        }
    }

    private void CastWave()
    {
        wave.Cast();
    }

    public void ForceDespawn(bool instant = false)
    {
        OnDespawn();
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