using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FlamingDrill : MonoBehaviour, IDespawneable
{
    float dmg;
    float time;
    public Vector2 dir;
    public AudioClip flamingDrillSound;
    public AudioClip blastSound;
    [SerializeField] Animator animator;

    [SerializeField] SpriteRenderer sprite;

    private float minDistance = 0.8f;

    bool firstHit;
    GroundFire lastfire;

    public void OnEnable()
    {
        firstHit = false;
        dmg = 45;
        time = 2f;
        CreateFireTrail();
    }

    public void PlayAnimation()
    {
        transform.localEulerAngles = new Vector3(0, 0, BulletBase.VectorToAngle(dir));
    }

    public void Update()
    {
        if (time > 0) time -= Time.deltaTime;
        else
        {
            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, GetType());
            return;
        }
        transform.position += (Vector3)dir * Time.deltaTime * 20f;

        if (Vector2.Distance(transform.position, lastfire.transform.position) > minDistance) CreateFireTrail();
    }

    void CreateFireTrail()
    {
        GroundFire nextFire = PoolManager.Get<GroundFire>();
        nextFire.transform.position = transform.position;
        lastfire = nextFire;
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
            foreach (PlayerItem item in Player.instance.equippedItems)
            {
                item.OnHit(Player.instance.equippedPassiveAbilities.Find(x => x.GetType() == typeof(PiercingShotAbility)), damage, enemy);
            }
            foreach (PlayerItem item in Player.instance.evolvedItems)
            {
                item.OnHit(Player.instance.equippedPassiveAbilities.Find(x => x.GetType() == typeof(PiercingShotAbility)), damage, enemy);
            }
            if (!firstHit)
            {
                FlamingDrillFlames blast = PoolManager.Get<FlamingDrillFlames>();
                blast.transform.position = transform.position;
                blast.transform.eulerAngles = transform.eulerAngles;
                AudioController.PlaySound(blastSound);

                firstHit = true;
            }
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