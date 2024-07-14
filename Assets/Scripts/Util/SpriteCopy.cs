using UnityEngine;

public class SpriteCopy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer toCopy;
    private SpriteRenderer spr;

    public void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }
    public void LateUpdate()
    {
        spr.color = toCopy.color;
        spr.sprite = toCopy.sprite;
    }
}