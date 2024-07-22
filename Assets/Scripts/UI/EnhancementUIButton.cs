using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancementUIButton : MonoBehaviour
{
    [SerializeField] EnhancementMenu menu;
    [SerializeField] TextMeshProUGUI title, description, type, rarity;
    [SerializeField] Image enhancementIcon, typeIcon;

    private Enhancement currentEnhancement;

    public void OnSelect()
    {
        AudioController.PlaySound(AudioController.instance.sounds.ui_select);
        menu.OnEnhancementSelect(currentEnhancement);
    }

    public void Display(Enhancement enhancement)
    {
        currentEnhancement = enhancement;
        title.text = enhancement.getName();

        int level = enhancement.getLevel() + 1;
        if (level > 1) title.text += " Lv. " + level;

        description.text = enhancement.GetDescription();
        type.text = enhancement.getType();

        enhancementIcon.sprite = enhancement.getIcon();

        switch (enhancement.getWeight())
        {
            default:
            case 1:
                rarity.text = "Super Rare";
                rarity.color = Color.yellow;
                break;
            case 2:
                rarity.text = "Rare";
                rarity.color = Color.blue;
                break;
            case 3:
                rarity.text = "Uncommon";
                rarity.color = Color.green;
                break;
            case 4:
                rarity.text = "Common";
                rarity.color = Color.gray;
                break;
        }

    }
}