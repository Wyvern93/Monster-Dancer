using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot :ITooltipDisplayable
{
    public CanvasGroup group;
    public int slot;
    public object source;

    public Image Icon;

    bool isDragging;

    public void Display(PlayerInventoryObject obj, int index)
    {
        slot = index;
        if (obj == null)
        {
            tooltipObj = null;
            Icon.color = Color.clear;
        }
        else
        {
            tooltipObj = obj;
            Icon.sprite = obj.GetIcon();
            Icon.color = Color.white;
        }
        
    }

    public void BeginDrag()
    {
        EnhancementMenu.instance.BeginDrag(this, source, slot);
        if (tooltipObj != null) Icon.color = new Color(1, 1, 1, 0.5f);
        isDragging = true;
    }

    public void EndDrag()
    {
        InventoryUISlot target = (InventoryUISlot)Tooltip.Instance.currentHoveredSlot;
        EnhancementMenu.instance.EndDrag(target);
        if (tooltipObj != null) Icon.color = Color.white;
        isDragging = false;
        Icon.transform.localPosition = Vector3.zero;
    }

    private void Awake()
    {
    }

    public void OnHover()
    {
        AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.ui_inventory_hover2);
        Tooltip.Instance.ShowTooltip(this, tooltipDirection);
    }

    public void OnUnHover()
    {
        //if (obj == null) return;
        Tooltip.Instance.HideTooltip(this);
    }
}