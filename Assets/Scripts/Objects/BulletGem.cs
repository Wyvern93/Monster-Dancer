using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGem : MonoBehaviour
{
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.name == "Player")
        {
            AudioController.PlaySound(Map.Instance.gemSound);
            Player.AddExp(1);
            PoolManager.Return(gameObject, GetType());
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "GemTrigger")
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, Time.deltaTime * speed);
            speed = Mathf.Clamp(speed + Time.deltaTime * 16f, 0, 64f);
        }
    }

}
