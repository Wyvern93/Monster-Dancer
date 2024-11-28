using System.Linq;
using UnityEngine;

public class EnhancementInventory : MonoBehaviour
{
    public InventoryUISlot[] slots;

    public void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            slots[i].slot = i;
            slots[i].source = this;
        }
    }

    public void OnEnable()
    {
        for (int i = 0; i < slots.Count(); i++)
        {
            slots[i].Display(null, i);
        }
    }

    public void UpdateSlots()
    {
        for(int i = 0; i < slots.Count(); i++)
        {
            slots[i].Display(Player.instance.inventory[i], i);
        }
    }
}