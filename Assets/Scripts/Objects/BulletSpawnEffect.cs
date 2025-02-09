using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawnEffect : MonoBehaviour
{
    int beats = 0;
    public Enemy source;
    bool isDespawning;
    [SerializeField] SpriteRenderer spriteRenderer;
    public float finalScale = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        isDespawning = false;
        beats = 0;
        finalScale = 0.5f;
        transform.localScale = Vector3.one * 1.5f;
        transform.eulerAngles = new Vector3(45f, 0, 0);
    }

    public void Despawn()
    {
        isDespawning = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * finalScale, Time.deltaTime * 2);
        transform.eulerAngles = new Vector3(45f, 0f, transform.eulerAngles.z + (30 * Time.deltaTime));
        if (BeatManager.isGameBeat) beats++;

        if (!source.gameObject.activeSelf && !isDespawning)
        {
            isDespawning = true;
        }

        if (source != null)
        {
            if (source.gameObject.activeSelf)
            {
                transform.position = source.transform.position;
            }
        }
        
        spriteRenderer.color = new Color(1, 1, 1, Mathf.MoveTowards(spriteRenderer.color.a, isDespawning ? 0f : 1f, Time.deltaTime * 2f));

        if (spriteRenderer.color.a <= 0 && isDespawning)
        {
            transform.parent = null;
            PoolManager.Return(gameObject, typeof(BulletSpawnEffect));
        }
    }
}
