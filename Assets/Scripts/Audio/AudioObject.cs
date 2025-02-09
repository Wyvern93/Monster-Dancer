using UnityEngine;

public class AudioObject : MonoBehaviour
{
    public AudioSource source;
    private void Update()
    {
        if (!source.isPlaying) PoolManager.Return(gameObject, GetType());
    }
}