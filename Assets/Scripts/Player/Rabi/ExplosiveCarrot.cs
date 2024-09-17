using System.Collections;
using UnityEngine;

public class ExplosiveCarrot : MonoBehaviour
{
    public Vector2 direction;
    public float height;
    public float force;

    [SerializeField] SpriteRenderer carrotSpr;
    [SerializeField] AudioSource spinSource;

    public float dmg;

    public void Init(Vector2 dir)
    {
        height = 0.3f;
        force = 15f;
        direction = dir;
        carrotSpr.transform.localEulerAngles = Vector3.zero;
        int level = (int)Player.instance.abilityValues["ability.carrotbarrage.level"];
        dmg = 30;
        StartCoroutine(Throw());
    }

    IEnumerator Throw()
    {
        //carrotSpr.sortingOrder = 3;
        spinSource.Play();
        Vector3 origin = transform.position;
        float angle = direction.x < 0 ? -800 : 800;
        while (height > 0.1f)
        {
            while (GameManager.isPaused) yield return new WaitForEndOfFrame();
            height += force * Time.deltaTime;
            force -= Time.deltaTime * 60f;
            carrotSpr.transform.localPosition = new Vector3(0, height, height * 2);
            carrotSpr.transform.localEulerAngles = new Vector3(0, 0, carrotSpr.transform.localEulerAngles.z + ((angle * Mathf.Abs(height)) * Time.deltaTime));
            force = Mathf.Clamp(force, -20f, 20f);
            height = Mathf.Clamp(height, 0, 10);
            transform.position = Vector3.Lerp(transform.position, origin + ((Vector3)direction * 2f), Time.deltaTime * 8f);
            
            yield return new WaitForEndOfFrame();
        }
        spinSource.Stop();
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
        //carrotSpr.sortingOrder = 2;

        CarrotExplosion carrotExplosion = PoolManager.Get<CarrotExplosion>();
        carrotExplosion.transform.position = transform.position;
        carrotExplosion.dmg = dmg;
        
        PoolManager.Return(gameObject, GetType());
        yield return null;
    }
}