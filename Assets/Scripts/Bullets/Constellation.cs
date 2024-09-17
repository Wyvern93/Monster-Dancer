using UnityEngine;

public class Constellation : MonoBehaviour
{
    [SerializeField] AudioClip starSpawn;
    [SerializeField] AudioClip ConstellationAttack;

    public void OnStarSpawn()
    {
        AudioController.PlaySound(starSpawn);
    }

    public void OnConstellationAttack()
    {
        AudioController.PlaySound(ConstellationAttack);
    }

    public void OnDespawn()
    {
        GetComponent<Animator>().Play("none");
    }
}