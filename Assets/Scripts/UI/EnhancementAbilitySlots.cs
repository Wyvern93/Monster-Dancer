using System.Linq;
using UnityEngine;

public class EnhancementAbilitySlots : MonoBehaviour
{
    public InventoryUISlot[] slots;

    public void Awake()
    {
        foreach (InventoryUISlot slot in slots)
        {
            slot.source = this;
        }
    }

    public void OnEnable()
    {

    }

    public void UpdateSlots()
    {
        for (int i = 0; i < slots.Count(); i++)
        {
            if (i > Player.instance.equippedPassiveAbilities.Count - 1)
            {
                slots[i].Display(null, i);
            }
            else
            {
                slots[i].Display(Player.instance.equippedPassiveAbilities[i], i);
            }
        }
    }
}