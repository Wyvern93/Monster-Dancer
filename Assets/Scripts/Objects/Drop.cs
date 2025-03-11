using UnityEngine;

public class Drop : MonoBehaviour
{
    public Animator animator;
    public bool followPlayer;
    protected float speed;
    private float timer = 0;
    public Vector2 dir;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }
    public void OnEnable()
    {
        timer = 0;
        Stage.Instance.dropsSpawned.Add(this);
        followPlayer = false;
        speed = 0;
    }

    private void Update()
    {
        if (GameManager.isPaused || UIManager.Instance.cutsceneManager.isInCutscene()) return;
        if (timer < 1) timer += Time.deltaTime;

        if (timer >= 1) followPlayer = true;

        if (dir.magnitude > 0)
        {
            transform.position += (Vector3)dir * 3f * Time.deltaTime;
            dir = Vector2.MoveTowards(dir, Vector2.zero, Time.deltaTime * 5f);
        }

        if (followPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, Time.deltaTime * speed);
            speed = Mathf.Clamp(speed + Time.deltaTime * 16f, 0, 64f);
        }
    }

    public virtual void ForceDespawn()
    {
        followPlayer = false;
        timer = 0;
        PoolManager.Return(gameObject, GetType());
    }
}