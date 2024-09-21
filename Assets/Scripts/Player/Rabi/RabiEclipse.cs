using UnityEngine;

public class RabiEclipse : MonoBehaviour, IDespawneable
{
    [SerializeField] AudioClip pulseSfx;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource sfxSource;
    int level;
    int dmg;
    float time;
    float healing;
    public void OnEnable()
    {
        animator.speed = 1f / BeatManager.GetBeatDuration() / 2F;
        level = (int)Player.instance.abilityValues["ability.eclipse.level"];
        string anim = level < 5 ? level < 2 ? "Eclipse" : "EclipseMedium" : "EclipseLong";
        animator.Play(anim);

        dmg = 12;
        time = 0;
        transform.parent = Player.instance.transform;
        transform.localPosition = Vector3.zero;
    }

    public void PlayPulse()
    {
        Player.TriggerCameraShake(0.3f, 0.3f);
        level = (int)Player.instance.abilityValues["ability.eclipse.level"];
        healing = level < 6 ? level < 4 ? 0.06f : 0.08f : 0.1f;
        int healnumber = (int)(Player.instance.currentStats.MaxHP * healing);
        Player.instance.CurrentHP = (int)Mathf.Clamp(Player.instance.CurrentHP + healnumber, 0, Player.instance.currentStats.MaxHP);
        UIManager.Instance.PlayerUI.UpdateHealth();
        UIManager.Instance.PlayerUI.SpawnDamageText(Player.instance.transform.position, healnumber, DamageTextType.Heal);
        AudioController.PlaySound(pulseSfx);
    }

    public void OnAnimationFinish()
    {
        Player.instance.despawneables.Remove(this);
        PoolManager.Return(gameObject, typeof(RabiEclipse));
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy.CanBeStunned(true)) enemy.OnStun(3);

            float damage = (int)(Player.instance.currentStats.Atk * dmg);  

            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;
            if (level >= 3) damage += (enemy.MaxHP * 0.05f);

            enemy.TakeDamage((int)damage, isCritical);
        }
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (level >= 7) bullet.Despawn();
            else bullet.OnStun(3);
        }
    }
    public void Update()
    {
        if (time <= BeatManager.GetBeatDuration())
        {
            sfxSource.volume = Mathf.MoveTowards(sfxSource.volume, 1f, Time.deltaTime / BeatManager.GetBeatDuration());
        }

        if (time >= 4 && level < 5)
        {
            sfxSource.volume = Mathf.MoveTowards(sfxSource.volume, 0, (Time.deltaTime / BeatManager.GetBeatDuration()) * 4f);
        }

        if (time >= 6 && level >= 5)
        {
            sfxSource.volume = Mathf.MoveTowards(sfxSource.volume, 0, (Time.deltaTime / BeatManager.GetBeatDuration()) * 4f);
        }
        time += Time.deltaTime;
    }

    public void ForceDespawn(bool instant = false)
    {
        StopAllCoroutines();
        PoolManager.Return(gameObject, GetType());
    }
}