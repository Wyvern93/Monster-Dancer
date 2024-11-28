using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnhancementAbilityEquipment : MonoBehaviour
{
    public InventoryUISlot[] slots;
    public List<Image> effects;
    public int abilityID;

    public void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            slots[i].slot = i;
            slots[i].source = this;
        }

        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].enabled = false;
        }
    }

    public void OnEnable()
    {
    }

    public void UpdateSlots()
    {
        if (Player.instance.equippedPassiveAbilities.Count > abilityID)
        {
            for (int i = 0; i < slots.Count(); i++)
            {
                effects[i].enabled = true;
                if (Player.instance.equippedPassiveAbilities[abilityID].equippedItems[i] == null) effects[i].enabled = false;
                slots[i].Display(Player.instance.equippedPassiveAbilities[abilityID].equippedItems[i], i);
            }
        }
        else
        {
            for (int i = 0; i < slots.Count(); i++)
            {
                effects[i].enabled = false;
                slots[i].Display(null, i);
            }
        }
        
    }
}