using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawnEffect : MonoBehaviour
{
    int beats = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        beats = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (BeatManager.isGameBeat) beats++;

        if (beats >= 1)
        {
            transform.parent = null;
            PoolManager.Return(gameObject, typeof(BulletSpawnEffect));
        }
    }
}
