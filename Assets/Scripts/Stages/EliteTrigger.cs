using UnityEngine;

public class EliteTrigger : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CameraWall"))
        {
            Stage.Instance.nextWaveIsElite = true;
            gameObject.SetActive(false);
        }
    }
}