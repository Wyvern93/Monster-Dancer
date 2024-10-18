using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIIcon : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI levelText;

    public void Display(Sprite sprite, int level, bool isMaxed, bool isItem)
    {
        image.sprite = sprite;
        if (sprite == null)
        {
            image.color = Color.clear;
            levelText.text = "";
        }
        else
        {
            image.color = Color.white;
            SetLevel(level, isMaxed, isItem);
        }
    }

    public void SetLevel(int level, bool isMaxed, bool isItem)
    {
        if (isMaxed) levelText.color = Color.yellow;
        else levelText.color = Color.white;

        if (isItem) levelText.text = $"x{level}";
        else levelText.text = $"LV {level}";

    }
}