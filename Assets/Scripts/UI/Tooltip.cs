using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;
    [SerializeField] CanvasGroup group;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI subtitle;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI tags;
    [SerializeField] Image icon;
    [SerializeField] Image tooltipBG;
    [SerializeField] Image downArrow;
    [SerializeField] Image leftrrow;
    [SerializeField] Image rightArrow;

    public ITooltipDisplayable currentHoveredSlot;
    private bool isTooltipVisible;

    private void Awake()
    {
        currentHoveredSlot = null;
        Instance = this;
    }

    public void ShowTooltip(ITooltipDisplayable displayable, Vector2 direction)
    {
        if (displayable.tooltipObj == null)
        {
            currentHoveredSlot = displayable;
            return;
        }
        if (displayable != currentHoveredSlot)
        {
            currentHoveredSlot = displayable;
            SetActive(true);
            SetTooltip(displayable.tooltipObj, displayable.transform.position, direction); // Assume Tooltip has a SetInfo method
            isTooltipVisible = true;
        }
    }

    private void SetActive(bool active)
    {
        if (!active)
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        else
        {
            group.alpha = 1;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    private Color WeightToRarity(int rarity)
    {
        switch (rarity)
        {
            case 1:
                return Color.yellow;
            case 2:
                return Color.blue;
            case 3:
                return Color.green;
            default:
            case 4:
                return Color.white;
        }
    }

    private string WeightRarityToName(int rarity)
    {
        if (rarity > 4) rarity = 4;
        if (rarity < 1) rarity = 1;
        switch (rarity)
        {
            case 1:
                return "Legendary";
            case 2:
                return "Epic";
            case 3:
                return "Rare";
            default:
            case 4:
                return "Common";
        }
    }

    private void SetTooltip(PlayerInventoryObject obj, Vector3 position, Vector2 direction)
    {
        if (obj is PlayerAbility)
        {
            PlayerAbility ability = obj as PlayerAbility;
            title.text = ability.getAbilityName();
            subtitle.text = (ability.isUltimate() ? "<color=#FF22FF>Special</color>" : "<color=#22FFFF>Ability</color>");
            description.text = ability.getAbilityDescription();
            tags.text = ability.getTags();
            icon.sprite = ability.GetIcon();
            tooltipBG.color = ability.GetTooltipColor();
        }

        if (obj is PlayerItem)
        {
            PlayerItem item = obj as PlayerItem;
            title.text = item.getItemName();
            subtitle.text = "<color=#FFFFFF>Item</color>";
            description.text = item.getItemDescription();
            tags.text = "";
            icon.sprite = item.GetIcon();
            tooltipBG.color = item.GetTooltipColor();
        }

        downArrow.color = tooltipBG.color;
        leftrrow.color = tooltipBG.color;
        rightArrow.color = tooltipBG.color;

        Vector3 basePosition = position;
        if (direction == Vector2.up)
        {
            downArrow.enabled = true;
            leftrrow.enabled = false;
            rightArrow.enabled = false;

            basePosition += Vector3.up * 3.5f;
        }

        if (direction == Vector2.left)
        {
            downArrow.enabled = false;
            leftrrow.enabled = false;
            rightArrow.enabled = true;

            basePosition -= Vector3.right * 2.3f;
        }

        if (direction == Vector2.right)
        {
            downArrow.enabled = false;
            leftrrow.enabled = true;
            rightArrow.enabled = false;

            basePosition += Vector3.right * 2.3f;
        }

        //basePosition.z = 0;
        //transform.position = new Vector3(basePosition.x, basePosition.y, 0);
        transform.position = basePosition;

    }

    private void Update()
    {
        if (currentHoveredSlot == null && isTooltipVisible)
        {
            isTooltipVisible = false;
            SetActive(false);
        }
    }

    public void HideTooltip(ITooltipDisplayable obj)
    {
        // Ensure we only hide if the current object is the one being exited
        if (currentHoveredSlot == obj)
        {
            currentHoveredSlot = null;
            Invoke(nameof(TryHideTooltip), 0.05f); // Delay to ensure another object isn't hovered this frame
        }
    }

    private void TryHideTooltip()
    {
        if (currentHoveredSlot == null)
        {
            SetActive(false);
            isTooltipVisible = false;
        }
    }
}