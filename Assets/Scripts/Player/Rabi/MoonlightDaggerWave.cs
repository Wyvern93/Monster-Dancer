using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonlightDaggerWave : MonoBehaviour, IDespawneable, IPlayerProjectile
{
    [SerializeField] AudioClip attackSound;
    [SerializeField] Animator animator;
    Vector3 dir;
    public float quarterbeats;
    public float velocity;
    public float dmg;
    public PlayerAbility abilitySource;

    public void OnEnable()
    {
        dir = Vector3.zero;
        animator.speed = 1f / BeatManager.GetBeatDuration() * 2f;
        AudioController.PlaySoundWithoutCooldown(attackSound);

        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;
        Vector2 animDir;

        animDir = new Vector2(difference.y, -difference.x);

        if (difference == Vector2.zero) difference = Vector2.right;
        transform.position = Player.instance.transform.position + ((Vector3)difference);
        transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, animDir));//getAngleFromVector(animDir));
        dir = difference;
    }
    public Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
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

    public void Update()
    {
        if (GameManager.isPaused) return;
        if (BeatManager.isQuarterBeat)
        {
            quarterbeats -= 0.25f;
        }

        if (quarterbeats <= 0)
        {
            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, GetType());
        }
        else
        {
            transform.position += (dir * velocity * Time.deltaTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = dmg;
            bool isCritical = abilitySource.GetCritChance() > Random.Range(0f, 100f);
            if (isCritical) damage *= 2.5f;
            enemy.TakeDamage((int)damage, isCritical);

            foreach (PlayerItem item in abilitySource.equippedItems)
            {
                if (item == null) continue;
                item.OnHit(abilitySource, damage, enemy, isCritical);
            }

            Vector2 dir = enemy.transform.position - Player.instance.transform.position;
            enemy.PushEnemy(dir, abilitySource.GetKnockback());
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }

    public void ForceDespawn(bool instant = false)
    {
        PoolManager.Return(gameObject, GetType());
    }
}
