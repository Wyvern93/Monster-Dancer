using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIIcon : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI levelText;

    public void Display(Sprite sprite, int level, bool isMaxed)
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
            SetLevel(level, isMaxed);
        }
    }

    public void SetLevel(int level, bool isMaxed)
    {
        if (isMaxed) levelText.color = Color.yellow;
        else levelText.color = Color.white;

        levelText.text = $"LV {level}";
    }

}