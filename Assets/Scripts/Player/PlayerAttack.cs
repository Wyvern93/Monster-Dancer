using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
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

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(Player.instance.currentStats.Atk);
        }
    }
}
