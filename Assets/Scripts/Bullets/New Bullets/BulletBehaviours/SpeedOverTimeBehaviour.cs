using UnityEngine;

public class SpeedOverTimeBehaviour : BulletBehaviour
{
    public float targetSpeed = 0f;
    public float speedPerBeat;
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        bullet.speed = Mathf.MoveTowards(bullet.speed, targetSpeed, speedPerBeat * (Time.deltaTime / BeatManager.GetBeatDuration() ));
    }
}