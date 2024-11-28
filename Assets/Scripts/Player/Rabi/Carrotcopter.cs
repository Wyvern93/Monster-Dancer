using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotCopter : MonoBehaviour, IDespawneable
{
    public PlayerAbility abilitySource;
    private Enemy currentTarget;
    private Vector2 targetPos;
    private float dmg;
    private float currentDistance;
    private float reach;
    [SerializeField] AudioClip shootSound;
    [SerializeField] GameObject shootFx;
    float despawnFXtime;
    public void OnEnable()
    {
        currentTarget = null;
    }
    public void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
        if (GameManager.isPaused) return;

        targetPos = UIManager.Instance.PlayerUI.crosshair.transform.position;

        if (BeatManager.isGameBeat) UpdateStats();
        MoveTowardsTarget();
        if (despawnFXtime > 0) despawnFXtime -= Time.deltaTime;
        else shootFx.SetActive(false);
    }

    private float GetAcceleration()
    {
        float currentDistance = Vector2.Distance(transform.position - (Vector3.up * 4f), targetPos) / 2.5f;
        currentDistance = Mathf.Clamp(currentDistance, 0, 2f);
        return currentDistance;
    }

    private void MoveTowardsTarget()
    {
        float acceleration = GetAcceleration() * abilitySource.GetSpeed();
        if (shootFx.activeSelf) acceleration *= 0.5f;
        if (GameManager.isPaused) return;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * acceleration);
        transform.position = new Vector3(transform.position.x, transform.position.y, 10);
    }

    public bool CanShoot()
    {
        Enemy e = Map.GetClosestEnemyTo(transform.position, reach);
        currentTarget = e;
        if (e == null) return false;
        return true;
    }

    public void Shoot()
    {
        Vector2 difference = (targetPos - (Vector2)transform.position).normalized;
        Vector2 animDir = new Vector2(difference.y, -difference.x);

        shootFx.transform.localPosition = difference * 0.25f;
        shootFx.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, animDir));
        ShootCarrot();
    }

    private void UpdateStats()
    {
        dmg = abilitySource.GetDamage();
        reach = abilitySource.GetReach();
    }


    public void ShootCarrot()
    {
        despawnFXtime = 0.17f;
        shootFx.SetActive(true);
        CarrotBullet carrot = PoolManager.Get<CarrotBullet>();
        carrot.abilitySource = abilitySource;
        carrot.transform.position = transform.position;
        carrot.isPiercing = false;
        carrot.dmg = dmg;
        carrot.abilitySource = Player.instance.equippedPassiveAbilities.Find(x => x.GetType() == typeof(CarrotcopterAbility));
        Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());

        Vector2 dir = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
        carrot.SetDirection(dir);

        AudioController.PlaySoundWithoutCooldown(shootSound, Random.Range(0.9f, 1.1f));
    }

    public void ForceDespawn(bool instant = false)
    {
        PoolManager.Return(gameObject, GetType());
    }
}