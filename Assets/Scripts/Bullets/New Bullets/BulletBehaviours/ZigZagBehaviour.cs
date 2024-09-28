using UnityEngine;

public class ZigZagBehaviour : BulletBehaviour
{
    bool isLeft;
    private float rotateAmount = 45f;

    public ZigZagBehaviour ()
    {
        isLeft = true;
    }
    public override void TriggerBehaviour(BulletBase bullet)
    {
        if (bullet.beat == 0) bullet.angle -= (rotateAmount / 2);
        if (isLeft) bullet.angle += rotateAmount;
        else bullet.angle -= rotateAmount;
        isLeft = !isLeft;
    }
}