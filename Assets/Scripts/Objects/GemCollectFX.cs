using UnityEngine;

public class GemCollectFX : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        PoolManager.Return(gameObject, typeof(GemCollectFX));
    }
}