using UnityEngine;

public class SpriteSpinBehaviour : BulletBehaviour
{
    public float spinSpeed = 180f;
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        bullet.transform.localEulerAngles = new Vector3(0, 0, bullet.transform.localEulerAngles.z + (Time.deltaTime * spinSpeed));
    }
}