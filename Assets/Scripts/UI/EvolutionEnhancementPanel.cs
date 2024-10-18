using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionEnhancementPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title, description, type;
    [SerializeField] Image enhancementIcon, typeIcon;

    private Enhancement currentEnhancement;

    public void Display(Enhancement enhancement)
    {
        currentEnhancement = enhancement;
        title.text = enhancement.getName();

        description.text = enhancement.GetDescription();
        type.text = enhancement.getDescriptionType();

        enhancementIcon.sprite = enhancement.getIcon();
    }
}