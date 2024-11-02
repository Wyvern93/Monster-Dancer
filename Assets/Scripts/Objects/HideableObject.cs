using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer spr;
    public void Hide()
    {
        spr.color = new Color(1, 1, 1, 0.5f);
    }

    public void UnHide()
    {
        spr.color = new Color(1, 1, 1, 1);
    }
}