using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TriggerOnDeathBehaviour : BulletBehaviour
{
    public System.Action<BulletBase> baseBullet;
    public TriggerOnDeathBehaviour(System.Action<BulletBase> baseBullet)
    {
        this.baseBullet = baseBullet;
    }
    public override void OnDespawn(BulletBase bullet)
    {
        baseBullet?.Invoke(bullet);
    }
}