using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class BlackHoleBehaviour : BulletBehaviour
{
    private List<BulletBase> bullets;

    public override void UpdateBehaviour(BulletBase bullet, float beatTime)
    {
        if (beatTime == 1) GetBullets(bullet.transform.position);

        foreach (BulletBase b in bullets)
        {
            Vector3 bulletdir = (bullet.transform.position - b.transform.position);
            float strength = Mathf.Clamp(4f - bulletdir.magnitude, 0f, 3f) / 2f;
            b.transform.position += bulletdir.normalized * strength * beatTime * Time.deltaTime;
        }
    }

    private void GetBullets(Vector3 origin)
    {
        if (bullets == null) bullets = new List<BulletBase>();
        bullets.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, 6f);
        foreach (Collider2D collider in colliders)
        {
            BulletBase bullet = collider.GetComponent<BulletBase>();
            if (bullet == null) continue;
            bullets.Add(bullet);
        }
    }
}