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

    public void SetAbilityIcon(Sprite sprite, bool isItem)
    {
        if (sprite == null)
        {
            icon.color = Color.black;
            frame.color = Color.black;
            cooldown = 0;
            cdImage.fillAmount = 0;
        }
        else
        {
            icon.color = Color.white;
            icon.sprite = sprite;
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
}