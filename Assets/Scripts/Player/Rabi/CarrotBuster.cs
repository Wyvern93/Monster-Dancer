using UnityEngine;

public class CarrotBuster : MonoBehaviour
{
    bool left = false;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip sfx;
    float abilityDamage;
    public void OnEnable()
    {
        transform.parent = Player.instance.transform;
        int level = (int)Player.instance.abilityValues["ability.carrotbuster.level"];
        abilityDamage = level < 4 ? level < 2 ? 12 : 18 : 24; // 200% 200%
        left = !left;
        animator.Play(left ? "carrotbuster_left" : "carrotbuster_right");
        AudioController.PlaySound(sfx, Random.Range(0.95f, 1.05f));

        Vector2 crosshairPos = UIManager.Instance.PlayerUI.crosshair.transform.position;
        Vector2 difference = (crosshairPos - (Vector2)Player.instance.transform.position).normalized;
        Vector2 animDir;

        animDir = new Vector2(difference.y, -difference.x);

        if (difference == Vector2.zero) difference = Vector2.right;
        transform.position = Player.instance.transform.position + ((Vector3)difference * 0.5f) + Vector3.up * 0.5f;
        transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, animDir));//getAngleFromVector(animDir));
    }

    public void OnFinish()
    {
        transform.parent = null;
        PoolManager.Return(gameObject, GetType());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            Vector2 dir = enemy.transform.position - Player.instance.transform.position;
            enemy.PushEnemy(dir, 5f);

            float damage = (int)(Player.instance.currentStats.Atk * abilityDamage);
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;
            enemy.TakeDamage((int)damage, isCritical);
        }

        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}