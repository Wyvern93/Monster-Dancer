using UnityEngine;

public class BeatSucessEffect : MonoBehaviour
{
    public Animator animator;
    public void OnAnimationFinish()
    {
        PoolManager.Return(gameObject, typeof(BeatSucessEffect));
    }
}