using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    public Animator animator;
    void OnEnable()
    {
        animator.speed = 1f / BeatManager.GetBeatDuration();
    }
    public void AnimationFinish()
    {
        PoolManager.Return(gameObject, typeof(SpawnEffect));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
