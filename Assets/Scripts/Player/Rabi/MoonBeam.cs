using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MoonBeam : MonoBehaviour
{
    float dotCD = 0.01f;

    [SerializeField] List<BoxCollider2D> boxColliders;
    [SerializeField] GameObject parts;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource sound;

    bool started = false;

    private float pitch = 1;
    private float maxPitch = 1.5f;
    private float size;

    private float abilityDamage = 10f;
    private float beamSpeed = 100;
    private bool stealHealth = false;
    public void OnBeamEnd()
    {
        started = false;
        foreach (BoxCollider2D boxCollider in boxColliders) boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        parts.SetActive(false);
        animator.Play("MoonBeam_Laser_End");
    }

    public void OnEnable()
    {
        OnInit();
    }

    public void OnInit()
    {
        int abilityLevel = (int)Player.instance.abilityValues["ability.moonbeam.level"];

        if (abilityLevel >= 2) abilityDamage = 12.5f;
        else abilityDamage = 10f;

        if (abilityLevel >= 3) beamSpeed = 125f;
        else beamSpeed = 100f;

        if (abilityLevel >= 4) stealHealth = true;
        else stealHealth = false;

        started = false;
        foreach (BoxCollider2D boxCollider in boxColliders) boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        parts.SetActive(false);
        sound.volume = 0;
        size = 5;
    }

    public void OnBeamStart()
    {
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)transform.position).normalized;
        Vector2 animDir;

        animDir = new Vector2(difference.y, -difference.x);
        Vector2 direction = difference;

        if (direction == Vector2.zero) direction = Vector2.right;
        transform.position = (Vector3)Player.position + new Vector3(0, 1.5f, 10f);

        //transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, animDir));
        pitch = 1;
        sound.pitch = pitch;
        spriteRenderer.enabled = true;
        parts.SetActive(true);
        started = true;
        foreach (BoxCollider2D boxCollider in boxColliders) boxCollider.enabled = true;
        animator.Play("MoonBeam_Laser_Start");
        sound.volume = 1;
    }

    public void Update()
    {

        // Read direction
        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)transform.position).normalized;
        Vector2 animDir;

        animDir = new Vector2(difference.y, -difference.x);
        Vector2 direction = difference;

        if (direction == Vector2.zero) direction = Vector2.right;
        transform.position = (Vector3)Player.position + new Vector3(0, 2f, 10f);
        float targetDir = Vector2.SignedAngle(Vector2.down, animDir);
        //transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(transform.localEulerAngles.z, targetDir, Time.deltaTime * 160f));
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + Time.deltaTime * 100f);
        //transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, animDir));//getAngleFromVector(animDir));

        if (!started && sound.volume > 0) sound.volume = Mathf.MoveTowards(sound.volume, 0, Time.deltaTime * 2f);
        if (!started) return;

        if (sound.pitch < maxPitch) sound.pitch = Mathf.MoveTowards(sound.pitch, maxPitch, Time.deltaTime / 5f);
        if (dotCD > 0)
        { 
            dotCD -= Time.deltaTime;
            //boxCollider.enabled = false;
        }
        else
        {
            //boxCollider.enabled = true;
            dotCD = 0f;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            bullet.Despawn();
        }
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = Player.instance.currentStats.Atk * abilityDamage;
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;
            enemy.TakeDamage((int)damage, isCritical);

            if (stealHealth)
            {
                float recovery = Mathf.Clamp(damage * 0.01f, 1f, 99999f);
                Player.instance.CurrentHP = (int)Mathf.Clamp(Player.instance.CurrentHP + recovery, 0, Player.instance.currentStats.MaxHP);
                UIManager.Instance.PlayerUI.SpawnDamageText(transform.position, (int)recovery, DamageTextType.Heal);
                UIManager.Instance.PlayerUI.UpdateHealth();
            }
        }
        
    }
}