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
    int level;
    private Vector2 origDir;

    public void Init(Vector3 dir)
    {
        level = (int)Player.instance.abilityValues["ability.carrotbarrage.level"];
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
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();

            carrotSpr.transform.localPosition = new Vector3(0, height, height * 2);
            carrotSpr.transform.localEulerAngles = new Vector3(0, 0, carrotSpr.transform.localEulerAngles.z + Time.deltaTime * 1200f);
            height = CalculateHeight(transform.position);
            transform.position = Vector3.MoveTowards(transform.position, end, Time.deltaTime * 8f);
            
            yield return new WaitForEndOfFrame();
        }
        
        if (!isSmall && level >= 7)
        {
            float angleDiff = 360f / 3f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 dir = BulletBase.angleToVector((angleDiff * i) + BulletBase.VectorToAngle(origDir));
                CastCarrot(dir, dmg, i == 0);
            }
        }

        CarrotExplosion carrotExplosion = PoolManager.Get<CarrotExplosion>();
        carrotExplosion.transform.position = transform.position;
        carrotExplosion.dmg = isSmall ? dmg * 0.5f : dmg;
        carrotExplosion.transform.localScale = Vector3.one * Player.instance.itemValues["explosionSize"];

        if (Player.instance.enhancements.Any(x => x.GetType() == typeof(FireworksKitItemEnhancement))) // This is never removed even with evolutions
        {
            bool doExplosion = Random.Range(0, 20) <= 0;
            if (doExplosion)
            {
                Vector2 explosionPos = transform.position + (Random.insideUnitSphere.normalized * 2f);

                CarrotExplosion carrotExplosion2 = PoolManager.Get<CarrotExplosion>();
                carrotExplosion2.transform.position = transform.position;
                carrotExplosion2.transform.localScale = transform.localScale * 0.5f * Player.instance.itemValues["explosionSize"];
                carrotExplosion2.dmg = isSmall ? dmg * 0.25f : dmg * 0.5f;
            }
        }

        PlayerCamera.TriggerCameraShake(0.6f, 0.3f);
        Player.instance.despawneables.Remove(this);
        PoolManager.Return(gameObject, GetType());
        yield return null;
    }

    public void CastCarrot(Vector2 direction, float damage, bool playSound)
    {
        ExplosiveCarrot carrot = PoolManager.Get<ExplosiveCarrot>();
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