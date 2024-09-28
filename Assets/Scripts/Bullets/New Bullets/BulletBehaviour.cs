using UnityEngine;

public class BulletBehaviour
{
    public int start, end;
    public bool triggerOnce = true;
    public void OnTrigger(BulletBase bullet)
    {
        if (bullet.beat != start && triggerOnce) return;
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

    public virtual void OnPlayerHit(BulletBase bullet) { }

}