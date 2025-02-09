using UnityEngine;

public class SpriteLookAngleBehaviour : BulletBehaviour
{
    public override void OnSpawn(BulletBase bullet)
    {
        bullet.transform.localEulerAngles = new Vector3(0, 0, bullet.angle);
    }
    public override void TriggerBehaviour(BulletBase bullet)
    {
        bullet.transform.localEulerAngles = new Vector3(0, 0, bullet.angle);
    }
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        bullet.transform.localEulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(bullet.transform.localEulerAngles.z, bullet.angle + (bullet.speed < 0 ? 180 : 0), Time.deltaTime * 320f));
    }
}