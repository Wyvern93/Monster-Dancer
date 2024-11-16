using System.Collections;
using UnityEngine;

public class CarrotJuiceBottle : MonoBehaviour, IDespawneable
{
    public Vector2 direction;
    public Vector3 start, end;
    private float height;
    private Vector2 origDir;
    [SerializeField] float maxHeight;

    [SerializeField] SpriteRenderer bottleSpr;

    public CarrotJuiceAbility ability;

    public void Init(Vector3 dir)
    {
        height = 0.3f;
        start = transform.position;
        end = start + dir;

        direction = dir;
        bottleSpr.transform.localEulerAngles = Vector3.zero;
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

            bottleSpr.transform.localPosition = new Vector3(0, height, height * 2);
            bottleSpr.transform.localEulerAngles = new Vector3(0, 0, bottleSpr.transform.localEulerAngles.z + Time.deltaTime * 1200f);
            height = CalculateHeight(transform.position);
            transform.position = Vector3.MoveTowards(transform.position, end, Time.deltaTime * 8f);

            yield return new WaitForEndOfFrame();
        }

        CarrotJuice carrotJuice = PoolManager.Get<CarrotJuice>();
        carrotJuice.transform.position = transform.position;
        Player.instance.despawneables.Add(carrotJuice.GetComponent<IDespawneable>());

        JuiceExplosion explosion = PoolManager.Get<JuiceExplosion>();
        explosion.transform.position = transform.position;

        Player.instance.despawneables.Remove(this);
        PoolManager.Return(gameObject, GetType());
        yield return null;
    }

    public void ForceDespawn(bool instant = false)
    {
        StopAllCoroutines();
        PoolManager.Return(gameObject, GetType());
    }
}