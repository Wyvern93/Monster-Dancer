using UnityEngine;

public class SpriteSpinBehaviour : BulletBehaviour
{
    public float spinSpeed = 180f;
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        bullet.spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, bullet.spriteRenderer.transform.localEulerAngles.z + (Time.deltaTime * spinSpeed));
    }
}