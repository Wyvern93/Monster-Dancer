using UnityEngine;
using static UnityEngine.UI.Image;

public class HomingToPlayerBehaviour : BulletBehaviour
{
    GameObject target;
    float amount = 1;
    public HomingToPlayerBehaviour(GameObject target, float amount = 1)
    {
        this.target = target;
        this.amount = amount;
    }
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        Vector3 playerPos = target.transform.position;
        Vector2 dir = (playerPos - bullet.transform.position).normalized;
        Vector2 finalDir = Vector2.Lerp(bullet.direction, dir, amount);

        bullet.angle = Vector2.SignedAngle(Vector2.down, finalDir) - 90;
    }
}