using System.Linq;
using UnityEngine;

public class GrazeTrigger : MonoBehaviour
{
    public RabbitReflexesAbility reflexAbility;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            AudioController.PlaySound(AudioController.instance.sounds.grazeSound);
            Player.AddSP(1);
            if (reflexAbility == null)
            {
                reflexAbility = (RabbitReflexesAbility)Player.instance.equippedPassiveAbilities.FirstOrDefault(x => x.GetType() == typeof(RabbitReflexesAbility));
            }
            
            if (reflexAbility != null)
            {
                collision.GetComponent<Bullet>().SetSuperGrazed();
            }
        }
    }
}