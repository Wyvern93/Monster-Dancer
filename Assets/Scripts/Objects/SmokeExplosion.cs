using UnityEngine;

public class SmokeExplosion : MonoBehaviour
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