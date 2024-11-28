using SpriteGlow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancementUIButton : ITooltipDisplayable
{
    [SerializeField] EnhancementMenu menu;
    [SerializeField] TextMeshProUGUI title, type, rarity;
    [SerializeField] Image enhancementIcon, bg;

    private Enhancement currentEnhancement;
    [SerializeField] SpriteGlowEffect glow;

    PlayerAbility ability;
    PlayerItem item;
    bool isAbility;


    public void OnSelect()
    {
        AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.ui_choice);
        menu.OnEnhancementSelect(currentEnhancement);
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

    public void OnHover()
    {
        AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.ui_inventory_hover);
        glow.OutlineWidth = 1;
        glow.GlowBrightness = 1.6f;
        Tooltip.Instance.ShowTooltip(this, tooltipDirection);
    }

    public void OnUnhover()
    {
        glow.OutlineWidth = 0;
        glow.GlowBrightness = 0;
        Tooltip.Instance.HideTooltip(this);
    }

    public void Display(Enhancement enhancement)
    {
        OnUnhover();
        if (enhancement.GetEnhancementType() == EnhancementType.Ability)
        {
            ability = enhancement.getAbility();
            isAbility = true;
        }
        else if (enhancement.GetEnhancementType() == EnhancementType.Item)
        {
            item = enhancement.getItem();
            isAbility = false;
        }

        if (isAbility)
        {
            tooltipObj = ability;
            title.text = ability.getAbilityName();
            type.text = ability.isUltimate() ? "Special" : "Ability";
            type.color = ability.isUltimate() ? new Color(1, 0.5f, 1) : new Color(0.5f, 1, 1);
            rarity.color = Color.clear;
            bg.color = ability.GetTooltipColor();
            glow.GlowColor = bg.color;
        }
        else
        {
            tooltipObj = item;
            title.text = item.getItemName();
            type.text = "Item";
            type.color = Color.white;
            rarity.color = WeightToRarity(enhancement.getWeight());
            rarity.text = WeightRarityToName(enhancement.getWeight());
            bg.color = rarity.color;
            glow.GlowColor = bg.color;
        }

        currentEnhancement = enhancement;
        enhancementIcon.sprite = enhancement.getIcon();
        glow.GlowBrightness = 0;
    }
}