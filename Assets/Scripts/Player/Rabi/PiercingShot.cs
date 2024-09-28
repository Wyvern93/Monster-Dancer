using System.Drawing;
using UnityEngine;

public class PiercingShot : MonoBehaviour, IDespawneable
{
    public bool isSmall;
    int level;
    float dmg;
    bool splitOnTouch;
    bool knockback;
    float time;
    public Vector2 dir;
    public AudioClip piercingShotSound;
    [SerializeField] Animator animator;

    [SerializeField] SpriteTrail trail;
    [SerializeField] SpriteRenderer sprite;

    public void OnEnable()
    {
        level = (int)Player.instance.abilityValues["ability.piercingshot.level"];
        dmg = level < 4 ? level < 2 ? 20f : 26f : 34f;
        splitOnTouch = level >= 7;
        knockback = level >= 6;
        time = 2f;
        trail.Play(sprite, 35, 0.06f, transform, new UnityEngine.Color(1, 0.7f, 0f, 0.5f), Vector3.zero);
    }

    public void PlayAnimation()
    {
        animator.Play(isSmall ? "PiercingShotSmall" : "PiercingShot");
        transform.localEulerAngles = new Vector3(0, 0, BulletBase.VectorToAngle(dir));
    }

    public void Update()
    {
        if (time > 0) time -= Time.deltaTime;
        else
        {
            Player.instance.despawneables.Remove(this);
            trail.Stop();
            trail.ForceDespawn();
            PoolManager.Return(gameObject, GetType());
            return;
        }

        transform.position += (Vector3)dir * Time.deltaTime * 20f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = (int)(Player.instance.currentStats.Atk * dmg * (isSmall ? 0.75f : 1f));
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)damage, isCritical);
            if (knockback)
            {
                Vector2 dir = enemy.transform.position - Player.instance.transform.position;
                enemy.PushEnemy(dir, 2f);
            }
            if (splitOnTouch)
            {
                trail.Stop();
                trail.ForceDespawn();
                float baseAngle = BulletBase.VectorToAngle(dir);
                Vector2 dir1 = BulletBase.angleToVector(baseAngle - 25f);
                Vector2 dir2 = BulletBase.angleToVector(baseAngle + 25f);

                PiercingShot shot1 = PoolManager.Get<PiercingShot>();
                shot1.dir = dir1;
                shot1.transform.position = transform.position;
                shot1.isSmall = true;
                shot1.PlayAnimation();
                Player.instance.despawneables.Add(shot1);

                PiercingShot shot2 = PoolManager.Get<PiercingShot>();
                shot2.dir = dir2;
                shot2.transform.position = transform.position;
                shot2.isSmall = true;
                shot2.PlayAnimation();
                Player.instance.despawneables.Add(shot2);

                Player.instance.despawneables.Remove(this);
                PoolManager.Return(gameObject, GetType());
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
        trail.Stop();
        trail.ForceDespawn();
        PoolManager.Return(gameObject, GetType());
    }
}