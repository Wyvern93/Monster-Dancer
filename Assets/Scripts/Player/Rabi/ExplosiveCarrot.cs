using System.Collections;
using System.Linq;
using UnityEngine;

public class ExplosiveCarrot : MonoBehaviour, IDespawneable
{
    public Vector3 start, end;
    private float height;
    [SerializeField] float maxHeight;

    public bool isSmall;

    [SerializeField] SpriteRenderer carrotSpr;

    public float dmg;
    private Vector2 origDir;
    public PlayerAbility abilitySource;

    public void Init(Vector3 dir)
    {
        transform.localScale = Vector3.one * (isSmall ? 0.75f : 1f);
        height = 0.3f;
        origDir = dir;
        start = transform.position;
        end = start + dir;

        carrotSpr.transform.localEulerAngles = Vector3.zero;
        StartCoroutine(Throw());
    }

    private float CalculateHeight(Vector2 position)
    {
        float distanceToEnemy = Vector3.Distance(position, end);
        float maxDistance = Vector3.Distance(start, end);
        float result = Mathf.Sin((distanceToEnemy / maxDistance) * Mathf.PI) * maxHeight;
        maxHeight = maxDistance / 2f;
        result = Mathf.Clamp(result, 0, maxHeight);

        return result;
    }

    IEnumerator Throw()
    {
        Vector3 origin = transform.position;
        while (Vector2.Distance(transform.position, end) > 0.01f)
        {
            while (GameManager.isPaused) yield return null;

            carrotSpr.transform.localPosition = new Vector3(0, height, height * 2);
            carrotSpr.transform.localEulerAngles = new Vector3(0, 0, carrotSpr.transform.localEulerAngles.z + Time.deltaTime * 1200f);
            height = CalculateHeight(transform.position);
            transform.position = Vector3.MoveTowards(transform.position, end, Time.deltaTime * 8f);
            
            yield return null;
        }

        CarrotExplosion carrotExplosion = PoolManager.Get<CarrotExplosion>();
        carrotExplosion.abilitySource = abilitySource;
        carrotExplosion.transform.position = transform.position;
        carrotExplosion.dmg = isSmall ? dmg * 0.5f : dmg;
        carrotExplosion.transform.localScale = Vector3.one * abilitySource.itemValues["explosionSize"];

        PlayerCamera.TriggerCameraShake(0.6f, 0.3f);
        Player.instance.despawneables.Remove(this);
        PoolManager.Return(gameObject, GetType());
        yield return null;
    }

    public void CastCarrot(Vector2 direction, float damage, bool playSound)
    {
        ExplosiveCarrot carrot = PoolManager.Get<ExplosiveCarrot>();
        carrot.abilitySource = abilitySource;
        carrot.transform.position = transform.position;
        carrot.dmg = damage;
        carrot.isSmall = true;
        carrot.Init(direction);
        Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());
    }

    public void ForceDespawn(bool instant = false)
    {
        StopAllCoroutines();
        PoolManager.Return(gameObject, GetType());
    }
}