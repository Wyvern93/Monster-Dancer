using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIIcon : MonoBehaviour
{
    public Image image;

    public void Display(Sprite sprite, bool isItem)
    {
        image.sprite = sprite;
        if (sprite == null)
        {
            image.color = Color.clear;
        }
        else
        {
            image.color = Color.white;
        }
    }
}