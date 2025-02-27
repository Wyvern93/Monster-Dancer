using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public abstract class Enhancement
{
    public abstract int getWeight();
    public virtual void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus) { }

    public abstract string getId();

    public abstract string getName();

    public abstract EnhancementType GetEnhancementType();

    public abstract string getDescriptionType();

    public virtual Sprite getIcon()
    {
        if (GetEnhancementType() == EnhancementType.Ability || GetEnhancementType() == EnhancementType.EvolvedAbility) return IconList.instance.getAbilityIcon(getId());
        else if (GetEnhancementType() == EnhancementType.Item) return IconList.instance.getItemIcon(getId());
        else return null;
    }

    public virtual void OnUpdate() { }

    public virtual PlayerAbility getAbility() { return null; }

    public virtual PlayerItem getItem() { return null; }
    
    public virtual Type getEvolutionItemType() { return null; }
    public virtual void OnEquip()
    {
        Player.instance.enhancements.Add(this);

        if (GetEnhancementType() == EnhancementType.Ability || GetEnhancementType() == EnhancementType.EvolvedAbility)
        {
            PlayerAbility ability = getAbility();
            if (getDescriptionType() == "Passive")
            {
                if (Player.instance.equippedPassiveAbilities.Count < 3)
                {
                    Player.instance.equippedPassiveAbilities.Add(ability);
                    UIManager.Instance.PlayerUI.SetPassiveIcon(getIcon(), Player.instance.getPassiveAbilityIndex(ability.GetType()));
                }
                else
                {
                    Player.instance.AddAbilityToInventory(ability);
                }
                
            }
            else if (getDescriptionType() == "Special")
            {
                Player.instance.ultimateAbility = ability;
                UIManager.Instance.PlayerUI.ShowSPBar();
                UIManager.Instance.PlayerUI.SetUltimateIcon(getIcon());
                ability.OnEquip();
                //UIManager.Instance.PlayerUI.UpdateSpecial();
            }
        }
        else if (GetEnhancementType() == EnhancementType.Item)
        {
            Player.instance.AddItemToInventory(getItem());
        }
        Player.instance.CalculateStats();
        BeatManager.instance.ClearBeatObjects();
    }

    public bool isUnlocked()
    {
        return SaveManager.PersistentSaveData.GetData<bool>($"enhancement.{getId()}.unlocked");
    }

    public virtual void OnEvolutionEquip(int slot)
    {
        Player.instance.enhancements.Add(this);
        Player.instance.equippedPassiveAbilities[slot] = getAbility();
        UIManager.Instance.PlayerUI.SetPassiveIcon(getIcon(), slot);
        Player.instance.CalculateStats();
    }

    public virtual bool isAvailable()
    {
        // PASSIVES
        if (GetEnhancementType() == EnhancementType.EvolvedAbility) return false;
        bool available = true;
        if (GetEnhancementType() == EnhancementType.Ability || GetEnhancementType() == EnhancementType.EvolvedAbility)
        {
            if (getDescriptionType() == "Passive")
            {
                if (Player.instance.equippedPassiveAbilities.Find(x => x.getId() == getId()) != null) available = false;
                if (Player.instance.DoesInventoryContain(getId())) available = false;
            }
            else if (getDescriptionType() == "Special")
            {
                available = false;
                if (Player.instance.ultimateAbility == null) available = true;
            }
        }
        else if (GetEnhancementType() == EnhancementType.Item)
        {
            available = true;
        }
        

        return available;
    }

    public abstract string GetDescription();
}