using UnityEngine;

public class SpriteLookAngleBehaviour : BulletBehaviour
{
    public override void TriggerBehaviour(BulletBase bullet)
    {
        bullet.spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, bullet.angle);
    }
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        bullet.spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(bullet.spriteRenderer.transform.localEulerAngles.z, bullet.angle + (bullet.speed < 0 ? 180 : 0), Time.deltaTime * 320f));
    }
}