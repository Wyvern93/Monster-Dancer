using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class EnhancementMenu : MonoBehaviour
{
    public static EnhancementMenu instance;
    public List<Enhancement> enhancements;
    [SerializeField] CanvasGroup choicesCanvasGroup;

    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] List<EnhancementUIButton> enhancementUIButtons;
    [SerializeField] Animator animator;
    [SerializeField] Button rerollButton;
    [SerializeField] TextMeshProUGUI rerollText;
    [SerializeField] GameObject moneyIcon;

    List<Enhancement> chosenEnhancements;
    [SerializeField] Button continueButton;

    [Header("Stats")]
    public TextMeshProUGUI maxHP;
    public TextMeshProUGUI Atk;
    public TextMeshProUGUI CritChance;
    public TextMeshProUGUI ExpMulti;
    public TextMeshProUGUI MovSpeed;

    [Header("Slot Containers")]
    public EnhancementInventory inventory;
    public EnhancementAbilityEquipment equipment1;
    public EnhancementAbilityEquipment equipment2;
    public EnhancementAbilityEquipment equipment3;
    public EnhancementAbilitySlots abilitySlot;
    public InventoryUISlot offInventorySlot;
    public InventoryUISlot trashSlot;

    [Header("Drag Data")]
    public object dragSource;
    public object dragTarget;
    public int origSlotIndex;
    public int targetSlotIndex;
    public InventoryUISlot dragSlot;
    public InventoryUISlot targetSlot;
    public Image ghostIcon;


    private int Rerolls;
    private int CurrentRerolls, rerollsUsed, rerollPrice;
    

    public void Awake()
    {
        instance = this;
        offInventorySlot.source = this;
        trashSlot.source = this;
        offInventorySlot.tooltipDirection = Vector2.right;
        equipment2.abilityID = 1;
        equipment3.abilityID = 2;
        animator.Play("EnhancementMenu_Closed");
    }

    public void SetOffInventory(PlayerInventoryObject obj)
    {
        offInventorySlot.Display(obj, 0);
        if (obj == null)
        {
            offInventorySlot.group.alpha = 0;
            offInventorySlot.group.interactable = false;
            offInventorySlot.group.blocksRaycasts = false;
            continueButton.interactable = true;
        }
        else
        {
            offInventorySlot.group.alpha = 1;
            offInventorySlot.group.interactable = true;
            offInventorySlot.group.blocksRaycasts = true;
            continueButton.interactable = false;
        }
    }

    public void BeginDrag(InventoryUISlot inventoryUISlot, object source, int index)
    {
        dragSource = source;
        origSlotIndex = index;
        dragSlot = inventoryUISlot;
        if (inventoryUISlot.tooltipObj != null)
        {
            ghostIcon.color = Color.white;
            ghostIcon.sprite = inventoryUISlot.Icon.sprite;
        }
    }

    public void EndDrag(InventoryUISlot inventoryUISlot)
    {
        ghostIcon.color = Color.clear;
        if (inventoryUISlot == null) return;
        dragTarget = inventoryUISlot.source;
        targetSlotIndex = inventoryUISlot.slot;
        targetSlot = inventoryUISlot;
        TrySwap();

        if (offInventorySlot.tooltipObj != null)
        {
            for (int i = 0; i < Player.instance.inventory.Length; i++)
            {
                if (Player.instance.inventory[i] == null)
                {
                    Player.instance.inventory[i] = offInventorySlot.tooltipObj;
                    SetOffInventory(null);
                    inventory.UpdateSlots();
                    break;
                }
            }
        }
    }

    public void SwapItems(PlayerAbility A, PlayerAbility B)
    {
        PlayerItem[] Aitems = new PlayerItem[6];
        PlayerItem[] Bitems = new PlayerItem[6];

        // We copy the items to be able to swap them
        for (int i = 0; i < 6; i++)
        {
            Aitems[i] = A.equippedItems[i];
            Bitems[i] = B.equippedItems[i];
        }

        // We unequip all items
        for (int i = 0; i < 6; i++)
        {
            if (A.equippedItems[i] != null) A.equippedItems[i].OnUnequip(A, i);
            if (B.equippedItems[i] != null) B.equippedItems[i].OnUnequip(B, i);

            A.equippedItems[i] = null;
            B.equippedItems[i] = null;
        }

        // We equip the items
        for (int i = 0; i < 6; i++)
        {
            A.equippedItems[i] = Bitems[i];
            B.equippedItems[i] = Aitems[i];
            if (A.equippedItems[i] != null) A.equippedItems[i].OnEquip(A, i);
            if (B.equippedItems[i] != null) B.equippedItems[i].OnEquip(B, i);
        }
    }

    private void TrySwap()
    {
        AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.ui_inventory_drag);
        if (dragSource is EnhancementMenu) return;
        // Try to swap from inventory
        if (dragTarget is EnhancementMenu)
        {
            // Trying to throw to trash
            if (targetSlot.name == "TrashSlot")
            {
                if (targetSlot.tooltipObj == null) return;
                if (dragSource is EnhancementInventory)
                {
                    Player.instance.inventory[origSlotIndex] = null;
                    inventory.UpdateSlots();
                }
                else if (dragSource is EnhancementAbilitySlots) return; // You can't trash equipped abilities
                else if (dragSource is EnhancementAbilityEquipment) // Trashing equipped items
                {
                    int abilityIndex = 0;
                    if ((EnhancementAbilityEquipment)targetSlot.source == equipment1) abilityIndex = 0;
                    else if ((EnhancementAbilityEquipment)targetSlot.source == equipment2) abilityIndex = 1;
                    else if ((EnhancementAbilityEquipment)targetSlot.source == equipment3) abilityIndex = 2;

                    PlayerAbility ability = Player.instance.equippedPassiveAbilities[abilityIndex];
                    ability.equippedItems[origSlotIndex].OnUnequip(ability, origSlotIndex);
                    Player.instance.equippedPassiveAbilities[abilityIndex].equippedItems[origSlotIndex] = null;

                    if (abilityIndex == 0) equipment1.UpdateSlots();
                    if (abilityIndex == 1) equipment2.UpdateSlots();
                    if (abilityIndex == 2) equipment3.UpdateSlots();

                    for (int i = 0; i < 3; i++)
                    {
                        if (i >= Player.instance.equippedPassiveAbilities.Count) continue;
                        UIManager.Instance.PlayerUI.SetPassiveIcon(Player.instance.equippedPassiveAbilities[i].GetIcon(), i);
                    }
                    PlayerAbility currentAbility = Player.instance.equippedPassiveAbilities[Player.instance.currentWeapon];
                    UIManager.Instance.PlayerUI.SetAmmoIcons(currentAbility.GetReloadIcon());

                    UIManager.Instance.PlayerUI.SetAmmo(currentAbility.currentAmmo, currentAbility.GetMaxAmmo());
                }
            }
            else return;
        }

        if (dragSource is EnhancementInventory) // From inventory
        {
            if (dragTarget is EnhancementInventory) // To inventory
            {
                PlayerInventoryObject first = dragSlot.tooltipObj;
                PlayerInventoryObject second = targetSlot.tooltipObj;

                Player.instance.inventory[targetSlotIndex] = first;
                Player.instance.inventory[origSlotIndex] = second;

                inventory.UpdateSlots();
            }
            if (dragTarget is EnhancementAbilitySlots) // Inventory to equip an ability
            {
                if (dragSlot.tooltipObj is not PlayerAbility) return; // You can't equip items as abilities

                PlayerAbility first = (PlayerAbility)targetSlot.tooltipObj;
                PlayerAbility second = (PlayerAbility)dragSlot.tooltipObj;

                // Change if currently selected
                if (targetSlotIndex == Player.instance.currentWeapon)
                {
                    Player.instance.equippedPassiveAbilities[targetSlotIndex].OnChange();
                }

                // Unequip the ability's items // EDIT HEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEERE
                Player.instance.equippedPassiveAbilities[targetSlotIndex] = second;
                Player.instance.inventory[origSlotIndex] = first; 

                SwapItems(first, second); // Swap items with the ability that swaps

                // Select if currently selected
                if (targetSlotIndex == Player.instance.currentWeapon)
                {
                    Player.instance.equippedPassiveAbilities[targetSlotIndex].OnSelect();
                }

                inventory.UpdateSlots();
                abilitySlot.UpdateSlots();

                for (int i = 0; i < 3; i++)
                {
                    if (i >= Player.instance.equippedPassiveAbilities.Count) continue;
                    UIManager.Instance.PlayerUI.SetPassiveIcon(Player.instance.equippedPassiveAbilities[i].GetIcon(), i);
                }
                PlayerAbility currentAbility = Player.instance.equippedPassiveAbilities[Player.instance.currentWeapon];
                UIManager.Instance.PlayerUI.SetAmmoIcons(currentAbility.GetReloadIcon());

                UIManager.Instance.PlayerUI.SetAmmo(currentAbility.currentAmmo, currentAbility.GetMaxAmmo());
            }
            if (dragTarget is EnhancementAbilityEquipment) // Inventory to equipment
            {
                if (dragSlot.tooltipObj is PlayerAbility) return; // You can't equip an ability as an equipment of an ability

                PlayerInventoryObject first = dragSlot.tooltipObj;
                PlayerInventoryObject second = targetSlot.tooltipObj;

                int abilityIndex = 0;
                if ((EnhancementAbilityEquipment)targetSlot.source == equipment1) abilityIndex = 0;
                else if ((EnhancementAbilityEquipment)targetSlot.source == equipment2) abilityIndex = 1;
                else if ((EnhancementAbilityEquipment)targetSlot.source == equipment3) abilityIndex = 2;

                if (abilityIndex > Player.instance.equippedPassiveAbilities.Count - 1) return; // You can't do anything if there's no ability

                PlayerAbility ability = Player.instance.equippedPassiveAbilities[abilityIndex];
                PlayerItem toEquipItem = (PlayerItem)dragSlot.tooltipObj;
                PlayerItem equippedItem = ability.equippedItems[targetSlotIndex];

                if (equippedItem != null)
                {
                    equippedItem.OnUnequip(ability, targetSlotIndex);
                    ability.equippedItems[targetSlotIndex] = null;
                }

                if (toEquipItem != null)
                {
                    ability.equippedItems[targetSlotIndex] = toEquipItem;
                    toEquipItem.OnEquip(ability, targetSlotIndex);
                }
                Player.instance.inventory[origSlotIndex] = second;

                inventory.UpdateSlots();

                if (abilityIndex == 0) equipment1.UpdateSlots();
                if (abilityIndex == 1) equipment2.UpdateSlots();
                if (abilityIndex == 2) equipment3.UpdateSlots();
            }
        }

        else if (dragSource is EnhancementAbilitySlots) // From ability slot
        {
            if (dragTarget is EnhancementMenu) return; // You can't trash or send to offinventory
            if (dragTarget is EnhancementAbilityEquipment) return; // You can't equip abilities as item equips
            if (dragTarget is EnhancementInventory) // Trying to send it to inventory
            {
                // You can only swap if the other slot is an ability
                if (targetSlot.tooltipObj is not PlayerAbility) return;

                PlayerAbility first = (PlayerAbility)dragSlot.tooltipObj;
                PlayerAbility second = (PlayerAbility)targetSlot.tooltipObj;

                // Change if currently selected
                if (origSlotIndex == Player.instance.currentWeapon)
                {
                    Player.instance.equippedPassiveAbilities[origSlotIndex].OnChange();
                }

                Player.instance.equippedPassiveAbilities[origSlotIndex] = (PlayerAbility)second;
                Player.instance.inventory[targetSlotIndex] = first;

                SwapItems(first, second); // Swap items with the ability that swaps

                // Select if currently selected
                if (origSlotIndex == Player.instance.currentWeapon)
                {
                    Player.instance.equippedPassiveAbilities[origSlotIndex].OnSelect();
                }

                abilitySlot.UpdateSlots();
                for (int i = 0; i < 3; i++)
                {
                    if (i >= Player.instance.equippedPassiveAbilities.Count) continue;
                    UIManager.Instance.PlayerUI.SetPassiveIcon(Player.instance.equippedPassiveAbilities[i].GetIcon(), i);
                }
                PlayerAbility currentAbility = Player.instance.equippedPassiveAbilities[Player.instance.currentWeapon];
                UIManager.Instance.PlayerUI.SetAmmoIcons(currentAbility.GetReloadIcon());

                UIManager.Instance.PlayerUI.SetAmmo(currentAbility.currentAmmo, currentAbility.GetMaxAmmo());

                inventory.UpdateSlots();
            }
            if (dragTarget is EnhancementAbilitySlots) // Swapping abilities
            {
                if (targetSlot.tooltipObj is null) return; // You can't swap with an ability not equipped

                PlayerAbility first = (PlayerAbility)dragSlot.tooltipObj;
                PlayerAbility second = (PlayerAbility)targetSlot.tooltipObj;

                // Change if currently selected
                if (origSlotIndex == Player.instance.currentWeapon)
                {
                    Player.instance.equippedPassiveAbilities[origSlotIndex].OnChange();
                }
                else if (targetSlotIndex == Player.instance.currentWeapon)
                {
                    Player.instance.equippedPassiveAbilities[targetSlotIndex].OnChange();
                }

                Player.instance.equippedPassiveAbilities[targetSlotIndex] = first;
                Player.instance.equippedPassiveAbilities[origSlotIndex] = second;

                SwapItems(first, second);

                // Change if currently selected
                if (origSlotIndex == Player.instance.currentWeapon)
                {
                    Player.instance.equippedPassiveAbilities[origSlotIndex].OnSelect();
                }
                else if (targetSlotIndex == Player.instance.currentWeapon)
                {
                    Player.instance.equippedPassiveAbilities[targetSlotIndex].OnSelect();
                }

                abilitySlot.UpdateSlots();

                for (int i = 0; i < 3; i++)
                {
                    if (i >= Player.instance.equippedPassiveAbilities.Count) continue;
                    UIManager.Instance.PlayerUI.SetPassiveIcon(Player.instance.equippedPassiveAbilities[i].GetIcon(), i);
                    
                }
                PlayerAbility currentAbility = Player.instance.equippedPassiveAbilities[Player.instance.currentWeapon];
                UIManager.Instance.PlayerUI.SetAmmoIcons(currentAbility.GetReloadIcon());

                UIManager.Instance.PlayerUI.SetAmmo(currentAbility.currentAmmo, currentAbility.GetMaxAmmo());
            }
        }
        else if (dragSource is EnhancementAbilityEquipment) // From ability equip
        {
            if (dragTarget is EnhancementMenu) return;
            if (dragTarget is EnhancementAbilitySlots) return; // You can't set an item to an ability
            if (dragTarget is EnhancementAbilityEquipment) // Swapping items between equipments
            {
                PlayerInventoryObject first = dragSlot.tooltipObj;
                PlayerInventoryObject second = targetSlot.tooltipObj;

                int firstAbility = 0;
                if ((EnhancementAbilityEquipment)dragSlot.source == equipment1) firstAbility = 0;
                else if ((EnhancementAbilityEquipment)dragSlot.source == equipment2) firstAbility = 1;
                else if ((EnhancementAbilityEquipment)dragSlot.source == equipment3) firstAbility = 2;

                int secondAbility = 0;
                if ((EnhancementAbilityEquipment)targetSlot.source == equipment1) secondAbility = 0;
                else if ((EnhancementAbilityEquipment)targetSlot.source == equipment2) secondAbility = 1;
                else if ((EnhancementAbilityEquipment)targetSlot.source == equipment3) secondAbility = 2;

                if (firstAbility > Player.instance.equippedPassiveAbilities.Count - 1) return; // You can't do anything if there's no ability
                if (secondAbility > Player.instance.equippedPassiveAbilities.Count - 1) return; // You can't do anything if there's no ability

                PlayerAbility A = Player.instance.equippedPassiveAbilities[firstAbility];
                PlayerAbility B = Player.instance.equippedPassiveAbilities[secondAbility];

                if (A.equippedItems[origSlotIndex] != null)
                    A.equippedItems[origSlotIndex].OnUnequip(A, origSlotIndex);
                if (B.equippedItems[targetSlotIndex] != null)
                    B.equippedItems[targetSlotIndex].OnUnequip(B, targetSlotIndex);

                A.equippedItems[origSlotIndex] = (PlayerItem)second;
                B.equippedItems[targetSlotIndex] = (PlayerItem)first;

                if (A.equippedItems[origSlotIndex] != null)
                    A.equippedItems[origSlotIndex].OnEquip(A, origSlotIndex);
                if (B.equippedItems[targetSlotIndex] != null)
                    B.equippedItems[targetSlotIndex].OnEquip(B, targetSlotIndex);

                if (firstAbility == 0 || secondAbility == 0) equipment1.UpdateSlots();
                if (firstAbility == 1 || secondAbility == 1) equipment2.UpdateSlots();
                if (firstAbility == 2 || secondAbility == 2) equipment3.UpdateSlots();
            }
            if (dragTarget is EnhancementInventory) // Equipment to inventory
            {
                if (targetSlot.tooltipObj is PlayerAbility) return; // You can't equip an ability as an equipment of an ability

                PlayerInventoryObject first = dragSlot.tooltipObj;
                PlayerInventoryObject second = targetSlot.tooltipObj;

                int abilityIndex = 0;
                if ((EnhancementAbilityEquipment)dragSlot.source == equipment1) abilityIndex = 0;
                else if ((EnhancementAbilityEquipment)dragSlot.source == equipment2) abilityIndex = 1;
                else if ((EnhancementAbilityEquipment)dragSlot.source == equipment3) abilityIndex = 2;

                if (abilityIndex > Player.instance.equippedPassiveAbilities.Count - 1) return; // You can't do anything if there's no ability

                PlayerAbility ability = Player.instance.equippedPassiveAbilities[abilityIndex];
                PlayerItem equippedItem = ability.equippedItems[origSlotIndex];
                PlayerItem toEquipItem = (PlayerItem)targetSlot.tooltipObj;

                if (equippedItem != null)
                {
                    equippedItem.OnUnequip(ability, origSlotIndex);
                    ability.equippedItems[origSlotIndex] = null;
                }

                if (toEquipItem != null)
                {
                    ability.equippedItems[origSlotIndex] = toEquipItem;
                    toEquipItem.OnEquip(ability, origSlotIndex);
                }
                Player.instance.inventory[targetSlotIndex] = equippedItem;

                inventory.UpdateSlots();

                if (abilityIndex == 0) equipment1.UpdateSlots();
                if (abilityIndex == 1) equipment2.UpdateSlots();
                if (abilityIndex == 2) equipment3.UpdateSlots();
            }
        }
    }

    public void Update()
    {
        if (canvasGroup.interactable == false) return;
        ghostIcon.transform.position = UIManager.Instance.PlayerUI.crosshair.transform.position;
    }

    public void Open()
    {
        UIManager.Instance.PlayerUI.OnOpenMenu();
        ghostIcon.color = Color.clear;
        inventory.UpdateSlots();
        abilitySlot.UpdateSlots();
        equipment1.UpdateSlots();
        equipment2.UpdateSlots();
        equipment3.UpdateSlots();

        chosenEnhancements = new List<Enhancement>();
        enhancements = new List<Enhancement>
        {
            FindEnhancement(1),
            FindEnhancement(2),
            FindEnhancement(3),
            FindEnhancement(4)
        };

        UpdateStats();

        DisplayEnhancements();
        animator.Play("EnhancementMenu_Open");
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
        GameManager.isPaused = true;
        if (!SaveManager.PersistentSaveData.ContainsKey("rerolls.level"))
        {
            SaveManager.PersistentSaveData.SetData("rerolls.level", 2);
            SaveManager.PersistentSaveData.SaveFile();
        }
        Rerolls = SaveManager.PersistentSaveData.GetData<int>("rerolls.level");
        rerollButton.interactable = Rerolls > 1;
        CurrentRerolls = Rerolls;
        rerollsUsed = 0;
        rerollText.text = $"Reroll ({CurrentRerolls})";
        rerollPrice = 0;
        moneyIcon.SetActive(false);
    }

    private void UpdateStats()
    {
        maxHP.text = $"MAX HP: {Player.instance.currentStats.MaxHP}";
        Atk.text = $"ATK: +{Mathf.Round((Player.instance.currentStats.Atk - 1) * 100f)}%";
        CritChance.text = $"CRIT CHANCE: {Player.instance.currentStats.CritChance}%";
        ExpMulti.text = $"EXP MULTI: +{Mathf.Round((Player.instance.currentStats.ExpMulti - 1) * 100f)}%";
        MovSpeed.text = $"MOV SPEED: +{Mathf.Round((Player.instance.currentStats.Speed - 1) * 100f)}%";
    }

    public void Close()
    {
        animator.Play("EnhancementMenu_Closed");
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(CloseMenuCoroutine());
    }

    protected IEnumerator CloseMenuCoroutine()
    {
        while (!BeatManager.menuGameBeat) yield return new WaitForEndOfFrame();
        GameManager.isPaused = false;
        Time.timeScale = 1.0f;
        UIManager.Instance.PlayerUI.OnCloseMenu();
    }

    public Enhancement FindEnhancement(int n)
    {
        int roll = Random.Range(0, 20);

        Enhancement enhancement;

        bool isSkill = roll > 8;

        if (isSkill) enhancement = GetSkillEnhancement();
        else enhancement = GetItemEnhancement();

        if (enhancement == null) enhancement = GetItemEnhancement();

        return enhancement;
    }

    private Enhancement GetSkillEnhancement()
    {
        List<Enhancement> random = new List<Enhancement>();
        List<Enhancement> owned = new List<Enhancement>();

        List<Enhancement> finalList = new List<Enhancement>();

        if (Player.instance.ultimateAbility != null) owned.AddRange(Player.instance.ultimateAbility.getEnhancementList());

        random = GameManager.runData.possibleSkillEnhancements;

        foreach (Enhancement enhancement in random)
        {
            for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
        }

        if (finalList.Count == 0) return null;

        finalList.RemoveAll(x => !x.isAvailable());
        foreach (Enhancement enhancement in chosenEnhancements)
        {
            finalList.RemoveAll(x => x.getId() == enhancement.getId());
        }
        if (finalList.Count == 0) return null;
        Enhancement finalEnhancement = null;
        int attempts = 50;

        while (finalEnhancement == null)
        {
            int n = Random.Range(0, finalList.Count - 1);
            if (chosenEnhancements.Any(x => x.getId() == finalList[n].getId()) == false)
            {
                finalEnhancement = finalList[n];
                chosenEnhancements.Add(finalEnhancement);
                return finalEnhancement;
            }
            attempts--;
            if (attempts == 0)
            {
                return null;
            } 
        }
        return null;
    }

    public void OnOpenAnimationEnd()
    {
        choicesCanvasGroup.interactable = true;
        choicesCanvasGroup.blocksRaycasts = true;
    }

    private Enhancement GetItemEnhancement()
    {
        List<Enhancement> random = new List<Enhancement>();

        List<Enhancement> finalList = new List<Enhancement>();

        random = GameManager.runData.possibleItemEnhancements;

        foreach (Enhancement enhancement in random)
        {
            for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
        }
        if (finalList.Count == 0) return null;

        Enhancement finalEnhancement = null;
        int attempts = 50;

        finalList.RemoveAll(x => !x.isAvailable());
        foreach (Enhancement enhancement in chosenEnhancements)
        {
            finalList.RemoveAll(x => x.getId() == enhancement.getId());
        }
        if (finalList.Count == 0) return null;

        while (finalEnhancement == null)
        {
            int n = Random.Range(0, finalList.Count - 1);

            if(chosenEnhancements.Any(x => x.getId() == finalList[n].getId()) == false)
            {
                finalEnhancement = finalList[n];
                chosenEnhancements.Add(finalEnhancement);
                return finalEnhancement;
            }
            attempts--;
            if (attempts == 0) return null;
        }
        return null;
    }

    private Enhancement GetStatEnhancement()
    {
        List<Enhancement> random = GameManager.runData.possibleStatEnhancements;
        List<Enhancement> finalList = new List<Enhancement>();

        foreach (Enhancement enhancement in random)
        {
            for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
        }
        if (finalList.Count == 0) return null;

        Enhancement finalEnhancement = null;
        int attempts = finalList.Count * 4;
        while (finalEnhancement == null)
        {
            int n = Random.Range(0, finalList.Count - 1);
            if (finalList[n].isAvailable())
            {
                if (chosenEnhancements.Any(x => x.GetType() == finalList[n].GetType()) == false)
                {
                    finalEnhancement = finalList[n];
                    chosenEnhancements.Add(finalEnhancement);
                    return finalEnhancement;
                }
            }
            attempts--;
            if (attempts == 0) return null;
        }
        return null;
    }


    public void DisplayEnhancements()
    {
        for (int i = 0; i < enhancementUIButtons.Count; i++)
        {
            enhancementUIButtons[i].Display(enhancements[i]);
        }
    }

    private List<Enhancement> ShuffleEnhancements(List<Enhancement> list)
    {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
        return list;
    }

    public void OnEnhancementSelect(Enhancement en)
    {
        en.OnEquip();
        FindEnhancement(enhancements.IndexOf(en) + 1);
        DisplayEnhancements();
        inventory.UpdateSlots();
        choicesCanvasGroup.interactable = false;
        choicesCanvasGroup.blocksRaycasts = false;
        if (offInventorySlot.tooltipObj == null)
        {
            offInventorySlot.group.alpha = 0;
            offInventorySlot.group.interactable = false;
            offInventorySlot.group.blocksRaycasts = false;
        }
        else
        {

        }
        inventory.UpdateSlots();
        abilitySlot.UpdateSlots();
        equipment1.UpdateSlots();
        equipment2.UpdateSlots();
        equipment3.UpdateSlots();
        animator.Play("EnhancementMenu_Inventory");
    }

    public void Reroll()
    {
        if (rerollPrice > 0) GameManager.runData.coins -= rerollPrice;
        UIManager.Instance.PlayerUI.coinText.text = GameManager.runData.coins.ToString();

        chosenEnhancements = new List<Enhancement>();
        enhancements = new List<Enhancement>
        {
            FindEnhancement(1),
            FindEnhancement(2),
            FindEnhancement(3),
            FindEnhancement(4)
        };

        DisplayEnhancements();
        if (!GameManager.isDebugMode) CurrentRerolls--;
        if (CurrentRerolls <= 0)
        {
            moneyIcon.SetActive(true);
        }
        rerollsUsed++;
        if (CurrentRerolls > 0)
        {
            rerollText.text = $"Reroll ({CurrentRerolls})";
        }
        else
        {
            if (rerollPrice == 0) rerollPrice = 50;
            else rerollPrice = (int)Mathf.Pow(rerollPrice, 1.2f);
            rerollText.text = $"Reroll ({rerollPrice})";
        }

        rerollButton.interactable = GameManager.runData.coins >= rerollPrice;
        EventSystem.current.SetSelectedGameObject(null);
    }
}