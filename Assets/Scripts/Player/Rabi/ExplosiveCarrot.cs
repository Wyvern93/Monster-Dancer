using System.Collections;
using System.Linq;
using UnityEngine;

public class ExplosiveCarrot : MonoBehaviour, IDespawneable
{
    public Vector2 direction;
    public float height;
    public float force;

    public bool isSmall;

    [SerializeField] SpriteRenderer carrotSpr;
    [SerializeField] AudioSource spinSource;

    public float dmg;
    int level;

    public void Init(Vector2 dir)
    {
        level = (int)Player.instance.abilityValues["ability.carrotbarrage.level"];
        transform.localScale = Vector3.one * (isSmall ? 0.75f : 1f);
        height = 0.3f;
        force = 15f;
        direction = dir;
        carrotSpr.transform.localEulerAngles = Vector3.zero;
        StartCoroutine(Throw());
    }
    public void PlaySpin()
    {
        spinSource.Play();
    }

    IEnumerator Throw()
    {
        //carrotSpr.sortingOrder = 3;
        Vector3 origin = transform.position;
        float angle = direction.x < 0 ? -800 : 800;
        float heightSpeed = isSmall ? 120f : 60f;
        while (height > 0.1f)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            height += force * Time.deltaTime;
            force -= Time.deltaTime * heightSpeed;
            carrotSpr.transform.localPosition = new Vector3(0, height, height * 2);
            carrotSpr.transform.localEulerAngles = new Vector3(0, 0, carrotSpr.transform.localEulerAngles.z + ((angle * Mathf.Abs(height)) * Time.deltaTime));
            force = Mathf.Clamp(force, -20f, 20f);
            height = Mathf.Clamp(height, 0, 10);
            transform.position = Vector3.Lerp(transform.position, origin + ((Vector3)direction * 2f), Time.deltaTime * 8f);
            
            yield return new WaitForEndOfFrame();
        }
        spinSource.Stop();
        /*
        int i = 20;
        float timer = 0.2f;
        float totaltimer = 0.2f;
        while (i > 0)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                totaltimer -= 0.02f;
                timer = totaltimer;
                if (i % 2 == 0) carrotSpr.color = Color.red;
                else carrotSpr.color = Color.white;
                i--;
            }
            yield return new WaitForEndOfFrame();
        }
        */
        //carrotSpr.sortingOrder = 2;

        if (!isSmall && level >= 7)
        {
            float angleDiff = 360f / 3f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 dir = BulletBase.angleToVector((angleDiff * i) + angle);
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
        if (playSound) carrot.PlaySpin();
        Player.instance.despawneables.Add(carrot.GetComponent<IDespawneable>());
    }

    public void ForceDespawn(bool instant = false)
    {
        StopAllCoroutines();
        PoolManager.Return(gameObject, GetType());
    }
}