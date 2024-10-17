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

    public virtual int getLevel()
    {
        if (GetEnhancementType() == EnhancementType.Ability ||GetEnhancementType() == EnhancementType.EvolvedAbility)
        {
            if (!Player.instance.abilityValues.ContainsKey($"ability.{getId()}.level")) return 0;
            else return (int)Player.instance.abilityValues[$"ability.{getId()}.level"];
        }
        else if (GetEnhancementType() == EnhancementType.Stat || GetEnhancementType() == EnhancementType.EvolutionItem)
        {
            return 0;
        }
        else if (GetEnhancementType() == EnhancementType.StackableItem)
        {
            if (!Player.instance.itemValues.ContainsKey($"item.{getId()}.level")) return 0;
            else return (int)Player.instance.itemValues[$"item.{getId()}.level"];
        }
        else
        {
            return 0;
        }
    }

    public abstract string getDescriptionType();

    public virtual Sprite getIcon()
    {
        if (GetEnhancementType() == EnhancementType.Ability || GetEnhancementType() == EnhancementType.EvolvedAbility) return IconList.instance.getAbilityIcon(getId());
        else if (GetEnhancementType() == EnhancementType.EvolutionItem || GetEnhancementType() == EnhancementType.StackableItem) return IconList.instance.getItemIcon(getId());
        else return null;
    }

    public virtual void OnUpdate() { }

    public virtual PlayerAbility getAbility() { return null; }

    public virtual PlayerItem getItem() { return null; }
    
    public virtual Type getEvolutionItemType() { return null; }
    public virtual void OnEquip()
    {
        Player.instance.enhancements.Add(this);
        int level = getLevel();
        if (level == 0)
        {
            if (GetEnhancementType() == EnhancementType.Ability || GetEnhancementType() == EnhancementType.EvolvedAbility)
            {
                Player.instance.abilityValues.Add($"ability.{getId()}.level", 1);
                if (getDescriptionType() == "Passive")
                {
                    Player.instance.equippedPassiveAbilities.Add(getAbility());
                    UIManager.Instance.PlayerUI.SetPassiveIcon(getIcon(), 1, false, Player.instance.getPassiveAbilityIndex(getAbility().GetType()));
                }
                else if (getDescriptionType() == "Active")
                {
                    Player.instance.activeAbility = getAbility();
                    UIManager.Instance.PlayerUI.SetActiveIcon(getIcon(), 1, false);
                }
                else if (getDescriptionType() == "Special")
                {
                    Player.instance.ultimateAbility = getAbility();
                    Player.AddSP(250);
                    UIManager.Instance.PlayerUI.ShowSPBar();
                    UIManager.Instance.PlayerUI.SetUltimateIcon(getIcon(), 1, false);
                    UIManager.Instance.PlayerUI.UpdateSpecial();
                }
            }
            else if (GetEnhancementType() == EnhancementType.EvolutionItem)
            {
                Player.instance.itemValues.Add($"item.{getId()}.level", 1);
                Player.instance.equippedItems.Add(getItem());
                UIManager.Instance.PlayerUI.SetItemIcon(getIcon(), 1, false, Player.instance.getItemIndex(getItem().GetType()));

            }
            else if (GetEnhancementType() == EnhancementType.StackableItem)
            {
                Player.instance.itemValues.Add($"item.{getId()}.level", 1);
                Player.instance.equippedItems.Add(getItem());
                UIManager.Instance.PlayerUI.SetItemIcon(getIcon(), 1, false, Player.instance.getItemIndex(getItem().GetType()));
            }
        }
        else
        {
            if (GetEnhancementType() == EnhancementType.Ability || GetEnhancementType() == EnhancementType.EvolvedAbility)
            {
                Player.instance.abilityValues[$"ability.{getId()}.level"] += 1;
                if (getDescriptionType() == "Passive")
                {
                    UIManager.Instance.PlayerUI.SetPassiveLevel(level + 1, level >= 7, Player.instance.getPassiveAbilityIndex(getAbility().GetType()));
                }
                else if (getDescriptionType() == "Active")
                {
                    UIManager.Instance.PlayerUI.SetActiveLevel(level + 1, level >= 4);
                }
                else if (getDescriptionType() == "Special")
                {
                    if (getLevel() == 6) Player.instance.MaxSP = (int)(Player.instance.MaxSP * 0.75f);
                    UIManager.Instance.PlayerUI.UpdateSpecial();
                    Player.instance.abilityValues[$"ability.{getId()}.level"] += 1;
                    UIManager.Instance.PlayerUI.SetUltimateLevel(level + 1, level >= 7);
                }
            }
            else
            {
                Player.instance.itemValues[$"item.{getId()}.level"] += 1;
                UIManager.Instance.PlayerUI.SetItemLevel(level + 1, false, Player.instance.getItemIndex(getItem().GetType()));
            }
            
        }
        Player.instance.CalculateStats();
    }

    public bool isUnlocked()
    {
        return SaveManager.PersistentSaveData.GetData<bool>($"enhancement.{getId()}.unlocked");
    }

    public virtual void OnEvolutionEquip(int slot)
    {
        Player.instance.enhancements.Add(this);
        Player.instance.abilityValues.Add($"ability.{getId()}.level", 1);
        Player.instance.equippedPassiveAbilities[slot] = getAbility();
        Player.instance.evolvedItems.Add(Player.instance.equippedItems.Find(x => x.GetType() == getEvolutionItemType()));
        Player.instance.equippedItems.RemoveAll(x=> x.GetType() == getEvolutionItemType());
        UIManager.Instance.PlayerUI.SetPassiveIcon(getIcon(), 1, false, slot);
        UIManager.Instance.PlayerUI.UpdateItemIcons();
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
                if (Player.instance.equippedPassiveAbilities.Count == 5) available = false;
                if (Player.instance.equippedPassiveAbilities.Find(x => x.getId() == getId()) != null)
                {
                    if (getLevel() < 7) available = true;
                    else available = false;
                }
                else
                {
                    if (getLevel() >= 7) available = false;
                }
            }
            else if (getDescriptionType() == "Active")
            {
                if (Player.instance.activeAbility == null) available = true;
                else
                {
                    if (Player.instance.activeAbility.getId() == getId())
                    {
                        if (getLevel() < 7) available = true;
                    }
                }
            }
            else if (getDescriptionType() == "Special")
            {
                available = false;
                if (Player.instance.ultimateAbility == null) available = true;
                else
                {
                    if (Player.instance.ultimateAbility.getId() == getId())
                    {
                        if (getLevel() < 7) available = true;
                    }
                }
            }
        }
        else if (GetEnhancementType() == EnhancementType.StackableItem)
        {
            if (Player.instance.equippedItems.Count == 6) available = false;
            if (Player.instance.equippedItems.Find(x => x.getId() == getId()) != null) available = true;
        }
        else if (GetEnhancementType() == EnhancementType.EvolutionItem)
        {
            if (Player.instance.equippedItems.Count == 6) available = false;
            if (Player.instance.equippedItems.Find(x => x.getId() == getId()) != null) available = false;
        }
        

        return available;
    }

    public abstract string GetDescription();
}