using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class BoxOfCarrots : MonoBehaviour, IDespawneable
{
    int level;
    float dmg;
    float heal;

    public void ForceDespawn(bool instant = false)
    {
        PoolManager.Return(gameObject, GetType());
    }

    public void OnEnable()
    {
        level = (int)Player.instance.abilityValues["ability.boxofcarrots.level"];
        dmg = level < 6 ? level < 3 ? 30 : 42 : 59;
        heal = level < 5 ? 0.05f : 0.10f;
    }

    public void Update()
    {
        if (Vector2.Distance(transform.position, Player.instance.transform.position) > 20)
        {
            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, GetType());
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            float damage = Player.instance.currentStats.Atk * dmg;
            bool isCritical = Player.instance.currentStats.CritChance > Random.Range(0f, 100f);
            if (isCritical) damage *= Player.instance.currentStats.CritDmg;

            enemy.TakeDamage((int)damage, isCritical);

            CarrotExplosion carrotExplosion = PoolManager.Get<CarrotExplosion>();
            carrotExplosion.transform.position = transform.position;
            carrotExplosion.dmg = dmg;
            carrotExplosion.transform.localScale = Vector3.one * 2f;

            Player.instance.despawneables.Remove(this);
            PoolManager.Return(gameObject, GetType());
        }

        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            int healnumber = (int)(Player.instance.currentStats.MaxHP * heal);
            Player.instance.CurrentHP = (int)Mathf.Clamp(Player.instance.CurrentHP + healnumber, 0, Player.instance.currentStats.MaxHP);
            UIManager.Instance.PlayerUI.UpdateHealth();
            UIManager.Instance.PlayerUI.SpawnDamageText(Player.instance.transform.position, healnumber, DamageTextType.Heal);
            Player.instance.despawneables.Remove(this);
            AudioController.PlaySound(AudioController.instance.sounds.superGrazeSound);
            PoolManager.Return(gameObject, GetType());
        }
    }
}