using UnityEngine;

public class Drop : MonoBehaviour
{
    public Animator animator;
    public bool followPlayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OnEnable()
    {
        Map.Instance.dropsSpawned.Add(this);
        followPlayer = false;
    }
    public virtual void ForceDespawn()
    {
        PoolManager.Return(gameObject, GetType());
    }
}