using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] SpriteRenderer spr_renderer;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] AudioClip attackSound;

    void Start()
    {
        spr_renderer.enabled = false;
        boxCollider.enabled = false;
    }

    public void Attack(Vector2 direction)
    {
        StartCoroutine(AttackCoroutine(direction));
    }

    public IEnumerator AttackCoroutine(Vector2 direction)
    {
        BeatManager.OnPlayerAction();

        float time = 0;

        // Read direction
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - Player.position).normalized;

        Debug.Log(difference);

        if (difference.x > 0.5f) direction.x = 1;
        else if (difference.x < -0.5f) direction.x = -1;
        else direction.x = 0;

        if (difference.y > 0.5f) direction.y = 1;
        else if (difference.y < -0.5f) direction.y = -1;
        else direction.y = 0;

        if (direction == Vector2.zero) direction = Vector2.right;
        transform.position = (Vector3)Player.position + (Vector3)direction;

        spr_renderer.enabled = true;
        boxCollider.enabled = true;

        // Sprite directions
        transform.localEulerAngles = new Vector3(0,0, getAngleFromVector(direction));

        AudioController.PlaySound(attackSound);

        while (time <= BeatManager.GetActionDuration())
        {
            time += Time.deltaTime;
            transform.position += ((Vector3)direction * 24f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        spr_renderer.enabled = false;
        boxCollider.enabled = false;

        Player.instance.isPerformingAction = false;
        yield break;
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

    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(1);
        }
    }
}
