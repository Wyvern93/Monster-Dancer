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
        menu.OnEnhancementSelect(currentEnhancement);
    }

    public void Display(Enhancement enhancement)
    {
        currentEnhancement = enhancement;
        title.text = enhancement.getName();

        int level = enhancement.getLevel();
        if (level > 1) title.text += " Lv. " + level;

        description.text = enhancement.GetDescription();
        type.text = enhancement.getType();

        enhancementIcon.sprite = enhancement.getIcon();
    }
}