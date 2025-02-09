using UnityEngine;

public class EnemySummonEffect : MonoBehaviour
{
    [SerializeField] AudioClip sound;

    public void OnEnable()
    {
        AudioController.PlaySound(sound);
    }
    public void OnAnimationEnd()
    {
        PoolManager.Return(gameObject, GetType());
    }
}