using UnityEngine;

public class BulletBehaviour
{
    public int start, end;
    public void OnTrigger(BulletBase bullet)
    {
        if (bullet.beat != start) return;
        TriggerBehaviour(bullet);
    }

    public virtual void TriggerBehaviour(BulletBase bullet)
    { }

    public virtual void OnUpdate(BulletBase bullet, float beatTime)
    {
        if (bullet.beat < start || (bullet.beat > end && end >= 0)) return;
        UpdateBehaviour(bullet, beatTime);
    }

    public virtual void UpdateBehaviour(BulletBase bullet, float beatTime)
    { }

}