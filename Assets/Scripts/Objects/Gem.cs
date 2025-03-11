using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Drop
{
    public int exp;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            AudioController.PlaySound(AudioController.instance.sounds.gemSound);
            Player.AddExp(exp);
            GemCollectFX fx = PoolManager.Get<GemCollectFX>();
            fx.transform.position = transform.position;
            PoolManager.Return(gameObject, GetType());
        }
        if (collision.name == "GemTrigger")
        {
            followPlayer = true;
        }
    }
}
