using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnBulletOnBeatBehaviour : BulletBehaviour
{
    public System.Action<BulletBase> baseBullet;
    public float delay;
    private float currentDelay;
    public SpawnBulletOnBeatBehaviour(System.Action<BulletBase> baseBullet, float delay)
    {
        this.delay = delay;//BeatManager.GetBeatDuration() / 3;
        this.baseBullet = baseBullet;
    }
    public override void TriggerBehaviour(BulletBase bullet)
    {
        
    }
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        if (currentDelay <= 0)
        {
            currentDelay = delay;
            baseBullet?.Invoke(bullet);
        }
        else
        {
            currentDelay -= Time.deltaTime;
        }
        
        if (beatTime == 1)
        {
            currentDelay = delay;
            baseBullet?.Invoke(bullet);
        }
    }
}