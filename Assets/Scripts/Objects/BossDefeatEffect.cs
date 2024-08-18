using UnityEngine;

public class BossDefeatEffect : MonoBehaviour
{
    public void OnEffectEnd()
    {
        Destroy(gameObject);
    }
}