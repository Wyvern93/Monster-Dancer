using System.Collections;
using UnityEngine;

public class CarrotJuiceBottle : MonoBehaviour
{
    public Vector2 direction;
    public float height;
    public float force;

    [SerializeField] SpriteRenderer bottleSpr;
    [SerializeField] AudioSource spinSource;

    public void Init(Vector2 dir)
    {
        height = 0.3f;
        force = 15f;
        direction = dir;
        bottleSpr.transform.localEulerAngles = Vector3.zero;
        StartCoroutine(Throw());
    }

    IEnumerator Throw()
    {
        //carrotSpr.sortingOrder = 3;
        spinSource.Play();
        Vector3 origin = transform.position;
        float angle = direction.x < 0 ? -800 : 800;

        float velocity = direction.magnitude;
        while (height > 0.1f)
        {
            height += force * Time.deltaTime;
            force -= Time.deltaTime * 60f;
            bottleSpr.transform.localPosition = new Vector3(0, height, height * 2);
            bottleSpr.transform.localEulerAngles = new Vector3(0, 0, bottleSpr.transform.localEulerAngles.z + ((angle * Mathf.Abs(height)) * Time.deltaTime));
            force = Mathf.Clamp(force, -20f, 20f);
            height = Mathf.Clamp(height, 0, 10);
            transform.position = Vector3.MoveTowards(transform.position, origin + ((Vector3)direction), Time.deltaTime * velocity);
            
            yield return new WaitForEndOfFrame();
        }
        spinSource.Stop();

        CarrotJuice carrotJuice = PoolManager.Get<CarrotJuice>();
        carrotJuice.transform.position = transform.position;
        
        PoolManager.Return(gameObject, GetType());
        yield return null;
    }
}