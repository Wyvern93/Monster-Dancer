using UnityEngine;
using static UnityEngine.UI.Image;

public class PoisonBehaviour : BulletBehaviour
{
    int amount;
    public PoisonBehaviour(int amount)
    {
        this.amount = amount;
    }
    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
    }

    public override void OnPlayerHit(BulletBase bullet)
    {
        Player.instance.poisonStatus += amount;
    }
}