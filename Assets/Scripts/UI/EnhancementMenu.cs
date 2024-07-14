using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EnhancementMenu : MonoBehaviour
{
    public static EnhancementMenu instance;
    public List<Enhancement> enhancements;

    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] List<EnhancementUIButton> enhancementUIButtons;

    public void Awake()
    {
        instance = this;
    }

    public void Open()
    {
        FindEnhancements();
        DisplayEnhancements();
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
        GameManager.isPaused = true;
    }

    public void Close()
    {
        Time.timeScale = 1.0f;
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        GameManager.isPaused = false;
    }

    public void FindEnhancements()
    {
        enhancements = new List<Enhancement>();
        
        foreach (Enhancement en in GameManager.runData.posibleEnhancements)
        {
            if (en.isAvailable())
            {
                enhancements.Add(en);
            }
        }
        enhancements = ShuffleEnhancements(enhancements);
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
        Close();
    }
}