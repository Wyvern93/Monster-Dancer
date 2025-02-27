using UnityEngine;

public class RabiEclipse : MonoBehaviour, IDespawneable
{
    [SerializeField] AudioClip pulseSfx;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource sfxSource;
    public float dmg;
    float time;
    public float healing;
    public void OnEnable()
    {
        animator.speed = 1f / BeatManager.GetBeatDuration() / 2F;
        string anim = "EclipseMedium";
        animator.Play(anim);

        time = 0;
        transform.parent = Player.instance.transform;
        transform.localPosition = Vector3.zero;
    }

    public void PlayPulse()
    {
        PlayerCamera.TriggerCameraShake(0.3f, 0.3f);
        int healnumber = (int)(Player.instance.currentStats.MaxHP * healing);
        Player.instance.Heal(healnumber);
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
            damage += (enemy.MaxHP * 0.05f);
            enemy.TakeDamage((int)damage, isCritical);
        }
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            bullet.Despawn();
        }
    }
    public void Update()
    {
        if (time <= BeatManager.GetBeatDuration())
        {
            sfxSource.volume = Mathf.MoveTowards(sfxSource.volume, 1f, Time.deltaTime / BeatManager.GetBeatDuration());
        }

        if (time >= 4)
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