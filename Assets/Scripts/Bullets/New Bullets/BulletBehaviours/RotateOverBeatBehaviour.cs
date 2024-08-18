using UnityEngine;

public class RotateOverBeatBehaviour : BulletBehaviour
{
    public float rotateAmount = 180f;
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        bullet.angle += rotateAmount * beatTime * Time.deltaTime;
    }
}