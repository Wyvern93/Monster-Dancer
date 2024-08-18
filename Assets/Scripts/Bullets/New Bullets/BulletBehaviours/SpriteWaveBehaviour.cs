using UnityEngine;

public class SpriteWaveBehaviour : BulletBehaviour
{
    const float angleDiff = 10;
    float sin;
    public override void TriggerBehaviour(BulletBase bullet)
    {
        bullet.spriteRenderer.transform.localEulerAngles = Vector3.zero;
    }
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        sin += Time.deltaTime * 360f * beatTime;
        float cos = Mathf.Cos(sin * Mathf.Deg2Rad) * angleDiff;
        bullet.spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, cos);
    }
}