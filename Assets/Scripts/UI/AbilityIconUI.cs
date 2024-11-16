using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconUI : MonoBehaviour
{
    public Image icon;
    public Image frame;
    [SerializeField] private Image cdImage;
    public float cooldown;
    [SerializeField] private Color frameColor;
    [SerializeField] TextMeshProUGUI levelText;

    public void SetAbilityIcon(Sprite sprite, int level, bool isMaxed, bool isItem)
    {
        if (sprite == null)
        {
            icon.color = Color.black;
            frame.color = Color.black;
            cooldown = 0;
            cdImage.fillAmount = 0;
            levelText.text = "";
        }
        else
        {
            icon.color = Color.white;
            icon.sprite = sprite;
            SetLevel(level, isMaxed, isItem);
        }
    }

    public void SetFrame(bool value)
    {
        frame.color = value ? frameColor : Color.black;
    }

    public void setCooldown(float value)
    {
        cooldown = value;
        cdImage.fillAmount = cooldown; 
    }

    public void SetLevel(int level, bool isMaxed, bool isItem)
    {
        if (isMaxed) levelText.color = Color.yellow;
        else levelText.color = Color.white;

        if (isItem) levelText.text = $"x{level}";
        else levelText.text = $"LV {level}";

    }
}