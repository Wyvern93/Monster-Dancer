using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Drop
{
    private float speed;
    public Vector2 dir;
    public int exp;
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
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            AudioController.PlaySound(AudioController.instance.sounds.gemSound);
            Player.AddExp(exp);
            PoolManager.Return(gameObject, GetType());
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!BeatManager.isPlaying) return;
        if (collision.name == "GemTrigger")
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, Time.deltaTime * speed);
            speed = Mathf.Clamp(speed + Time.deltaTime * 16f, 0, 64f);
        }
    }

}
