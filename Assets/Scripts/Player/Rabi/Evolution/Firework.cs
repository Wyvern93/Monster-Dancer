using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class Firework : MonoBehaviour, IDespawneable
{
    public Vector3 start, end;
    public Enemy target;
    private float height;
    [SerializeField] float maxHeight;

    [SerializeField] Transform fireworkTransform;

    private Vector2 direction;
    [SerializeField] float speed;
    [SerializeField] float startingTime;

    public void OnEnable()
    {
        startingTime = 4f;
        direction = transform.up;
        speed = 5;
        fireworkTransform.localEulerAngles = new Vector3(0, 0, BulletBase.VectorToAngle(Vector2.up));
    }

    public void Update()
    {
        if (start == null || target == null || fireworkTransform == null) return;
        speed = Mathf.MoveTowards(speed, 15f, Time.deltaTime * 4f);
        if (target.gameObject.activeSelf)
        {
            end = target.transform.position;
        }
        Vector2 targetDir = (end - transform.position).normalized;
        direction = Vector2.Lerp(direction, targetDir, Time.deltaTime * 10);
        transform.position = transform.position + (Vector3)(direction * speed * Time.deltaTime);
        height = CalculateHeight(transform.position);
        float height2 = CalculateHeight((Vector2)transform.position + (direction * 0.1f));
        Vector2 position1 = new Vector2(transform.position.x, transform.position.y + height);
        Vector2 position2 = new Vector2(transform.position.x + (direction.x * 0.1f), transform.position.y + (direction.y * 0.1f) + height2);
        Vector2 spriteDirection = (position2 - position1).normalized;

        fireworkTransform.localPosition = new Vector3(0, height, height * 4);
        fireworkTransform.localEulerAngles = new Vector3(0, 0,BulletBase.VectorToAngle(spriteDirection));

        if (Vector3.Distance(transform.position, end) < 0.2f) Explode();

        if (startingTime > 0) startingTime -= Time.deltaTime;
        else Explode();
    }

    void Explode()
    {
        FireworkExplosion explosion = PoolManager.Get<FireworkExplosion>();
        explosion.dmg = 65;
        explosion.canSpawnMini = true;
        explosion.transform.position = transform.position;
        explosion.transform.localScale = Vector3.one * Player.instance.itemValues["explosionSize"];
        PoolManager.Return(gameObject, typeof(Firework));
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

    public void ForceDespawn(bool instant = false)
    {
        PoolManager.Return(gameObject, GetType());
    }
}