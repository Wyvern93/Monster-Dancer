using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Drop
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            AudioController.PlaySound(AudioController.instance.sounds.coinSound);
            GameManager.runData.coins += 10;
            UIManager.Instance.PlayerUI.coinText.text = GameManager.runData.coins.ToString();
            PoolManager.Return(gameObject, GetType());
        }
        if (collision.name == "GemTrigger")
        {
            followPlayer = true;
        }
    }
}
