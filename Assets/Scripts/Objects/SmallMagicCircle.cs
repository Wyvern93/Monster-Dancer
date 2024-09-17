using UnityEngine;

public class SmallMagicCircle : MonoBehaviour
{
    public bool visible;
    [SerializeField] SpriteRenderer spriteRenderer;
    private Color color;
    public int despawnTime;
    private int beat;
    private bool despawning;
    public void OnEnable()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0);
        color = Color.white;
        visible = true;
        despawning = false;
        beat = 0;
    }

    public void Despawn()
    {
        despawning = true;
        visible = false;
        color = new Color(1, 1, 1, 0);
        transform.parent = null;
    }

    public void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + (90f * Time.deltaTime));
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, color, Time.deltaTime * 8f);
        if (BeatManager.isGameBeat && !GameManager.isPaused) beat++;
        if (beat >= despawnTime && !despawning) Despawn();

        if (despawning && spriteRenderer.color.a <= 0.01f) PoolManager.Return(gameObject, typeof(SmallMagicCircle));
    }
}