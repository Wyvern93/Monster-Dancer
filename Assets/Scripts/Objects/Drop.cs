using UnityEngine;

public class Drop : MonoBehaviour
{
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OnEnable()
    {
        Map.Instance.dropsSpawned.Add(this);
    }
    public virtual void ForceDespawn()
    {
        PoolManager.Return(gameObject, GetType());
    }
}