using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionPanelIcon : MonoBehaviour
{
    [SerializeField] EvolutionUI menu;
    [SerializeField] Image icon;
    public bool isAbility, isAttack;
    [SerializeField] Button button;

    private PlayerAbility playerAbility;
    private PlayerAttack playerAttack;
    private PlayerItem playerItem;
    [SerializeField] GameObject spark;

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }

    public void SetSpark(bool state)
    {
        spark.SetActive(state);
    }

    public void OnSelect()
    {
        if (!button.interactable) return;
        AudioController.PlaySound(AudioController.instance.sounds.ui_select);
        if (isAttack)
        {
            menu.OnAttackSelect(this);
        }
        else if (isAbility)
        {
            menu.OnAbilitySelect(playerAbility, this);
        }
        else
        {
            menu.OnItemSelect(playerItem, this);
        }
    }

    public void Display(PlayerAbility ability)
    {
        isAttack = false;
        isAbility = true;
        playerAbility = ability;
        icon.sprite = ability.GetIcon();
        icon.color = Color.white;
        button.interactable = true;
    }

    public void Clear(bool isAbility)
    {
        isAttack = false;
        this.isAbility = isAbility;
        icon.sprite = null;
        icon.color = Color.clear;
        button.interactable = false;
        playerAbility = null;
        playerItem = null;
    }

    public void Display(PlayerItem item)
    {
        isAttack = false;
        isAbility = false;
        icon.sprite = item.GetIcon();
        icon.color = Color.white;
        button.interactable = true;
        playerItem = item;
    }

    public void Display(Sprite atkspr)
    {
        icon.sprite = atkspr;
        icon.color = Color.white;
        button.interactable = true;
        isAttack = true;
        isAbility = false;
    }
}