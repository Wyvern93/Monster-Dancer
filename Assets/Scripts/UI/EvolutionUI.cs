using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvolutionUI : MonoBehaviour
{
    public static EvolutionUI instance;
    public EvolutionEnhancementPanel enhancementPanel;

    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] List<EvolutionPanelIcon> abilityIcons, itemIcons;
    [SerializeField] Image selectedAbility, selectedItem, evolvedResult;
    [SerializeField] Animator animator;
    [SerializeField] Button evolveButton, backButton;
    [SerializeField] TextMeshProUGUI evolveText, backText;

    private PlayerAbility ability;
    private PlayerItem item;
    private Enhancement enhancement;
    private bool evolutionDone;

    [SerializeField] Transform magicCircle1, magicCircle2;
    [SerializeField] float angle, orbitDistance, orbitSpeed;

    private bool evolving;
    public void Awake()
    {
        instance = this;
        animator.Play("EvolutionMenu_Closed");
        orbitDistance = 50;
        angle = 180;
        orbitSpeed = 0;
    }

    public void Open()
    {
        evolving = false;
        evolutionDone = false;
        evolveButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        evolveButton.interactable = false;
        backButton.interactable = true;
        enhancementPanel.gameObject.SetActive(false);
        selectedAbility.color = Color.clear;
        selectedItem.color = Color.clear;
        evolvedResult.color = Color.clear;
        ability = null;
        item = null;

        UpdateIcons();
        animator.Play("EvolutionMenu_Open");
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
        GameManager.isPaused = true;
        magicCircle1.transform.localPosition = new Vector2(-50, 0);
        magicCircle2.transform.localPosition = new Vector2(50, 0);
        orbitDistance = 50;
        angle = 180;
        orbitSpeed = 0;
        ActivateSparks();
    }

    private void UpdateIcons()
    {
        abilityIcons[0].Display(IconList.instance.getAbilityIcon("moonlightdaggers"));
        for (int i = 0; i < abilityIcons.Count - 1; i++) 
        {
            if (i < Player.instance.equippedPassiveAbilities.Count)
            {
                abilityIcons[i + 1].Display(Player.instance.equippedPassiveAbilities[i]);
            }
            else
            {
                abilityIcons[i + 1].Clear(true);
            }

        }

        for (int i = 0; i < itemIcons.Count; i++)
        {
            if (i < Player.instance.equippedItems.Count) itemIcons[i].Display(Player.instance.equippedItems[i]);
            else itemIcons[i].Clear(false);
        }
    }

    public void Close()
    {
        //animator.Play("EvolutionMenu_Closed");
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(CloseMenuCoroutine());
        if (evolutionDone) Map.Instance.fairyCage.gameObject.SetActive(false);
    }

    protected IEnumerator CloseMenuCoroutine()
    {
        while (!BeatManager.menuGameBeat) yield return new WaitForEndOfFrame();
        GameManager.isPaused = false;
        Time.timeScale = 1.0f;
    }

    public void OnAttackSelect(EvolutionPanelIcon icon)
    {
        selectedAbility.color = Color.clear;
    }

    public void OnAbilitySelect(PlayerAbility ability, EvolutionPanelIcon icon)
    {
        if (ability == null) return;
        selectedAbility.color = Color.white;
        selectedAbility.sprite = ability.GetIcon();
        this.ability = ability;
        evolveButton.interactable = CheckCompatibility();
    }

    public void OnItemSelect(PlayerItem item, EvolutionPanelIcon icon)
    {
        if (item == null) return;
        selectedItem.color = Color.white;
        selectedItem.sprite = item.GetIcon();
        this.item = item;
        evolveButton.interactable = CheckCompatibility();
    }

    public bool CheckCompatibility()
    {
        if (ability == null) return false;
        if (item == null) return false;
        if (ability.isEvolved()) return false;
        if (item.GetType() == ability.getEvolutionItemType()) return true;
        return false;
    }

    public void OnEvolutionClick()
    {
        ability.OnDespawn();
        DeactivateSparks();
        evolveButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);

        foreach (EvolutionPanelIcon icon in abilityIcons)
        {
            icon.SetInteractable(false);
        }

        foreach (EvolutionPanelIcon icon in itemIcons)
        {
            icon.SetInteractable(false);
        }
        // DO ANIMATION
        animator.Play("EvolutionCombine");
        evolving = true;
        AudioController.PlaySound(AudioController.instance.sounds.ui_evolve, side: true);
        //OnEvolutionFinish();
    }
    public void OnEvolutionFinish()
    {
        evolving = false;
        animator.Play("EvolutionFinish");
        selectedItem.color = Color.clear;
        selectedAbility.color = Color.clear;
        evolvedResult.color = Color.white;
        backButton.gameObject.SetActive(true);
        backButton.interactable = true;

        enhancementPanel.Display(enhancement);
        enhancement.OnEvolutionEquip(Player.instance.getPassiveAbilityIndex(ability.GetType()));
        enhancementPanel.gameObject.SetActive(true);
        evolutionDone = true;
    }

    public void Update()
    {
        if (evolving)
        {
            orbitSpeed += Time.unscaledDeltaTime * 1200;
            orbitDistance = Mathf.MoveTowards(orbitDistance, 0, Time.unscaledDeltaTime * 25);
            angle += orbitSpeed * Time.unscaledDeltaTime;
            magicCircle1.transform.localPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * orbitDistance, (Mathf.Sin(angle * Mathf.Deg2Rad) * orbitDistance));
            magicCircle2.transform.localPosition = new Vector2(Mathf.Cos((angle + 180f) * Mathf.Deg2Rad) * orbitDistance, (Mathf.Sin((angle + 180f) * Mathf.Deg2Rad) * orbitDistance));
        }
    }

    public void SetEvolutionResultIcon()
    {
        enhancement = ability.getEvolutionEnhancement();
        evolvedResult.sprite = enhancement.getIcon();
    }

    private void ActivateSparks()
    {
        Debug.Log("Checking to activate sparks");
        foreach (EvolutionPanelIcon icon in abilityIcons)
        {
            icon.SetSpark(false);
        }

        foreach (EvolutionPanelIcon icon in itemIcons)
        {
            icon.SetSpark(false);
        }
        for (int i = 0; i < 5; i++)
        {
            if (i < Player.instance.equippedPassiveAbilities.Count)
            {
                PlayerAbility ability = Player.instance.equippedPassiveAbilities[i];
                PlayerItem item = Player.instance.equippedItems.FirstOrDefault(x => x.GetType() == ability.getEvolutionItemType());

                if (ability.isEvolved()) continue;
                if (item == null) continue;

                if (item.GetType() == ability.getEvolutionItemType())
                {
                    abilityIcons[i + 1].SetSpark(true);
                    int itemId = Player.instance.equippedItems.IndexOf(item);
                    itemIcons[itemId].SetSpark(true);
                }
            }


        }
    }
    private void DeactivateSparks()
    {
        foreach (EvolutionPanelIcon icon in abilityIcons)
        {
            icon.SetSpark(false);
        }

        foreach (EvolutionPanelIcon icon in itemIcons)
        {
            icon.SetSpark(false);
        }
    }
}