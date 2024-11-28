using UnityEngine;

public class LunarRainRay : MonoBehaviour
{
    public float cooldown;
    public float dmg;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioClip sound;
    public LunarRainAbility abilitySource;
    float alpha;
    public void OnEnable()
    {
        AudioController.PlaySound(sound, Random.Range(0.8f, 1.2f));
    }

    public void OnAnimationEnd()
    {
        PoolManager.Return(gameObject, GetType());
    }

    public void Update()
    {
        
    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FairyCage"))
        {
            FairyCage cage = collision.GetComponent<FairyCage>();
            cage.OnHit();
        }
    }
}