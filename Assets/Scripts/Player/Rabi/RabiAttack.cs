using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RabiAttack : PlayerAttack
{
    [SerializeField] AudioClip attackSound;
    [SerializeField] Animator animator;
    int pierce;
    bool hasExploded;

    public override void Start()
    {
        base.Start();
    }

    public override void Attack(Vector2 direction)
    {
        int level = (int)Player.instance.abilityValues["attack.moonlightdaggers.level"];
        pierce = (int)Player.instance.abilityValues["Attack_Pierce"];
        hasExploded = false;
        StartCoroutine(AttackCoroutine(direction));
    }

    public IEnumerator AttackCoroutine(Vector2 direction)
    {
        animator.speed = 1.0f / BeatManager.GetBeatDuration();

        float time = 0;

        // Read direction
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;
        Vector2 animDir;
        float spread = Player.instance.abilityValues["Attack_Spread"];
        spread = Random.Range(-spread, spread);
        difference = rotate(difference, spread);

        animDir = new Vector2(difference.y, -difference.x);
        direction = difference;

        if (direction == Vector2.zero) direction = Vector2.right;
        transform.position = Player.instance.transform.position + (Vector3)direction;

        spr_renderer.enabled = true;
        boxCollider.enabled = true;

        // Sprite directions
        transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, animDir));//getAngleFromVector(animDir));

        AudioController.PlaySound(attackSound);

        float velocity = 10f * Player.instance.abilityValues["Attack_Velocity"];
        
        dir = direction;

        float abilityDuration = Player.instance.abilityValues["Attack_Time"];
        while (time <= abilityDuration)
        {
            time += Time.deltaTime;
            transform.position += ((Vector3)dir * velocity * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Player.instance.isPerformingAction = false;

        

        PoolManager.Return(gameObject, GetType());
        yield break;
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

    public override void Update()
    {
        
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (pierce <= 0)
        {
            Player.instance.isPerformingAction = false;
            PoolManager.Return(gameObject, GetType());
            return;
        }
        
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = Player.instance.abilityValues["Attack_Damage"] * Player.instance.currentStats.Atk;
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;
            enemy.TakeDamage((int)damage, isCritical);
            pierce--;
            if (!hasExploded && Player.instance.abilityValues["Attack_Explode"] == 1)
            {
                MoonlightShockwave shockwave = PoolManager.Get<MoonlightShockwave>();
                shockwave.transform.position = transform.position;
            }
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
            pierce--;
        }
    }
}
