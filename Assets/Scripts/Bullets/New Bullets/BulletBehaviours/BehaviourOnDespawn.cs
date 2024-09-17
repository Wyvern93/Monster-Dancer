using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourOnDespawn : BulletBehaviour
{
    public System.Action<BulletBase> baseBullet;
    public BehaviourOnDespawn(System.Action<BulletBase> baseBullet)
    {
        this.baseBullet = baseBullet;
    }
    public override void TriggerBehaviour(BulletBase bullet)
    {

    }
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        if (bullet.lifetime == 0 && beatTime == 1)
        {
            baseBullet?.Invoke(bullet);
        }
    }
}