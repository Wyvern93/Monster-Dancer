using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Drop
{
    public Vector2 dir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dir.magnitude > 0)
        {
            transform.position += (Vector3)dir * 3f * Time.deltaTime;
            dir = Vector2.MoveTowards(dir, Vector2.zero, Time.deltaTime * 5f);
        }

        if (followPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, Time.deltaTime * speed);
            speed = Mathf.Clamp(speed + Time.deltaTime * 16f, 0, 64f);
        }
    }

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
