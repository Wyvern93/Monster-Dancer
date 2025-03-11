using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Drop
{

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            AudioController.PlaySound(AudioController.instance.sounds.healSound);
            Player.instance.CurrentHP = (int)Mathf.Clamp(Player.instance.CurrentHP + 10, 0, Player.instance.currentStats.MaxHP);
            UIManager.Instance.PlayerUI.UpdateHealth();
            UIManager.Instance.PlayerUI.SpawnDamageText(Player.instance.transform.position, 10, DamageTextType.Heal);

            PoolManager.Return(gameObject, GetType());
        }
        if (collision.name == "GemTrigger")
        {
            followPlayer = true;
        }
    }
}
