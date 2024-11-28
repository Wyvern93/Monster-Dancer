using System.Linq;
using UnityEngine;

public class GrazeTrigger : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            AudioController.PlaySound(AudioController.instance.sounds.grazeSound);
        }
    }
}