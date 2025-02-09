using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CarrotDeliveryTruck : MonoBehaviour, IDespawneable
{
    public int numbeats;
    public int lap, maxlaps;
    public float truckDmg;
    public float bulletDmg; 
    public float velocity;
    public float cd, maxcd;

    [SerializeField] AudioClip shootSound;

    public CarrotDeliveryAbility ability;

    private bool facingRight = true;
    List<float> offsets = new List<float>()
    {
        2f,
        -2f,
        4f,
        -4f,
        3f,
        -3f,
        5f,
        -5f
    };
    public void OnEnable()
    {
        maxlaps = 4;
        facingRight = true;
        transform.position = new Vector3(Player.instance.transform.position.x - 12, Player.instance.transform.position.y + offsets[0], 0);
        lap = 0;
        transform.localScale = Vector3.one;
        maxcd = BeatManager.GetBeatDuration() / 8;
    }

    public void Update()
    {
        if (GameManager.isPaused) return;
        if (transform.position.x > Player.instance.transform.position.x + 12 && facingRight)
        {
            facingRight = false;
            lap++;
            if (lap >= maxlaps)
            {
                Player.instance.despawneables.Remove(this);
                PoolManager.Return(gameObject, GetType());
            }
            else transform.position = new Vector3(Player.instance.transform.position.x + 12, Player.instance.transform.position.y + offsets[lap], 0);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (transform.position.x < Player.instance.transform.position.x - 12 && !facingRight)
        {
            facingRight = true;
            lap++;
            if (lap >= maxlaps) PoolManager.Return(gameObject, GetType());
            else transform.position = new Vector3(Player.instance.transform.position.x - 12, Player.instance.transform.position.y + offsets[lap], 0);
            transform.localScale = Vector3.one;
        }
        transform.position += (facingRight ? Vector3.right : Vector3.left) * velocity * Time.deltaTime;

        if (cd <= 0)
        {
            ShootCarrot();
            cd = maxcd;
        }
        else
        {
            cd -= Time.deltaTime;
        }
    }

    public void ShootCarrot()
    {
        CarrotBullet carrot = PoolManager.Get<CarrotBullet>();
        carrot.abilitySource = ability;
        carrot.transform.position = transform.position;
        carrot.isPiercing = false;
        Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());

        Enemy e = Stage.GetRandomEnemy();
        int attempts = 5;
        while (e == null && attempts > 0)
        {
            e = Stage.GetRandomEnemy();
            attempts--;
        }
        if (e == null) return;

        Vector2 dir = ((Vector2)e.transform.position - (Vector2)transform.position).normalized;
        carrot.SetDirection(dir);
        carrot.dmg = bulletDmg;

        AudioController.PlaySound(shootSound, Random.Range(0.9f, 1.1f));
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            enemy.TakeDamage((int)truckDmg, false);
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