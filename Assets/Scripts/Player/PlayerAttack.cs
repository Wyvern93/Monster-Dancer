using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour, IDespawneable
{
    [SerializeField] protected SpriteRenderer spr_renderer;
    [SerializeField] protected BoxCollider2D boxCollider;
    public Vector2 dir;

    public virtual void Start()
    {
    }

    public virtual void Attack(Vector2 direction)
    {
    }

    public virtual void Update()
    {
        
    }

    public virtual Sprite GetIcon() { return null; }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = Player.instance.currentStats.Atk;
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)damage, isCritical);
        }
    }

    public void ForceDespawn(bool instant = false)
    {
        PoolManager.Return(gameObject, GetType());
    }
}
