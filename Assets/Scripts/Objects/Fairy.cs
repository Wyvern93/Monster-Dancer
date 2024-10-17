using UnityEngine;

public class Fairy : MonoBehaviour
{
    [SerializeField] FairyCage cage;
    [SerializeField] CircleCollider2D circleCollider;
    public bool isOpened;

    public Vector2 position, targetPosition;
    public float movementDelay;

    public void OnEnable()
    {
        circleCollider.enabled = false;
        position = transform.position;
        isOpened = false;
    }

    public void OnCageBreak()
    {
        isOpened = true;
        circleCollider.enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject.name == "Player")
        {
            Player.instance.OpenEvolutionMenu();
        }
    }

    public void Update()
    {
        if (!isOpened) return;

        if (movementDelay <= 0 || (Vector2)transform.position == targetPosition)
        {
            movementDelay = 0.3f;
            targetPosition = position + ((Vector2)Random.insideUnitCircle * 0.4f);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime / 2f);
    }
}