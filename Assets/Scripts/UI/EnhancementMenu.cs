using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnhancementMenu : MonoBehaviour
{
    public static EnhancementMenu instance;
    public List<Enhancement> enhancements;

    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] List<EnhancementUIButton> enhancementUIButtons;
    [SerializeField] Animator animator;
    [SerializeField] Button rerollButton;
    [SerializeField] TextMeshProUGUI rerollText;
    [SerializeField] GameObject moneyIcon;

    List<Enhancement> chosenEnhancements;

    [Header("Stats")]
    public TextMeshProUGUI maxHP;
    public TextMeshProUGUI Atk;
    public TextMeshProUGUI CritChance;
    public TextMeshProUGUI ExpMulti;
    public TextMeshProUGUI MovSpeed;


    private int Rerolls;
    private int CurrentRerolls, rerollsUsed, rerollPrice;
    

    public void Awake()
    {
        instance = this;
        animator.Play("EnhancementMenu_Closed");
    }

    public void Open()
    {
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
    }

    public Enhancement FindEnhancement(int n)
    {
        int roll = Random.Range(0, 20);

        Enhancement enhancement;

        bool isThirdOption = roll == 0;
        bool isSkill = roll >= 1 && roll <= 10;
        bool isItem = roll > 10;

        if (isThirdOption)
        {
            if (n < 3) enhancement = GetStatEnhancement();
            else if (n == 3) enhancement = new BonusHealEnhancement();
            else enhancement = new BonusCoinsEnhancement();
            return enhancement;
        } 
        else if (isSkill) enhancement = GetSkillEnhancement();
        else enhancement = GetItemEnhancement();

        if (isSkill && enhancement == null) GetItemEnhancement();
        if (isItem && enhancement == null)
        {
            if (n < 3) enhancement = GetStatEnhancement();
            else if (n == 3) enhancement = new BonusHealEnhancement();
            else enhancement = new BonusCoinsEnhancement();
            return enhancement;
        }
        if (enhancement == null)
        {
            if (n < 3) enhancement = GetStatEnhancement();
            else if (n == 3) enhancement = new BonusHealEnhancement();
            else enhancement = new BonusCoinsEnhancement();
        }
        
        return enhancement;
    }

    private Enhancement GetSkillEnhancement()
    {
        List<Enhancement> random = new List<Enhancement>();
        List<Enhancement> owned = new List<Enhancement>();

        List<Enhancement> finalList = new List<Enhancement>();

        foreach (PlayerAbility ability in Player.instance.equippedPassiveAbilities)
        {
            owned.AddRange(ability.getEnhancementList());
        }

        if (Player.instance.activeAbility != null) owned.AddRange(Player.instance.activeAbility.getEnhancementList());
        if (Player.instance.ultimateAbility != null) owned.AddRange(Player.instance.ultimateAbility.getEnhancementList());
        owned.AddRange(Player.instance.GetAttackEnhancementList());

        bool isOwned = Random.Range(0, 10) <= 5;
        if (isOwned)
        {
            foreach (Enhancement enhancement in owned)
            {
                for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
            }
            if (finalList.Count == 0)
            {
                random = GameManager.runData.possibleSkillEnhancements;
                foreach (Enhancement enhancement in random)
                {
                    for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
                }
            }
        }
        else
        {
            random = GameManager.runData.possibleSkillEnhancements;
            foreach (Enhancement enhancement in owned) random.Remove(enhancement);

            foreach (Enhancement enhancement in random)
            {
                for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
            }
        }
        if (finalList.Count == 0) return null;


        finalList.RemoveAll(x => !x.isAvailable());
        if (finalList.Count == 0) return null;
        Enhancement finalEnhancement = null;
        int attempts = 50;

        while (finalEnhancement == null)
        {
            int n = Random.Range(0, finalList.Count - 1);
            if (chosenEnhancements.Any(x => x.GetType() == finalList[n].GetType()) == false)
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

    private Enhancement GetItemEnhancement()
    {
        List<Enhancement> random = new List<Enhancement>();
        List<Enhancement> owned = new List<Enhancement>();

        List<Enhancement> finalList = new List<Enhancement>();

        foreach (PlayerItem item in Player.instance.playerItems)
        {
            owned.AddRange(item.getEnhancementList());
        }

        bool isOwned = Random.Range(0, 10) <= 5;

        if (isOwned)
        {
            foreach (Enhancement enhancement in owned)
            {
                for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
            }
            if (finalList.Count == 0)
            {
                random = GameManager.runData.possibleItemEnhancements;

                foreach (Enhancement enhancement in random)
                {
                    for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
                }
            }
        }
        else
        {
            random = GameManager.runData.possibleItemEnhancements;
            foreach (Enhancement enhancement in owned) random.Remove(enhancement);

            foreach (Enhancement enhancement in random)
            {
                for (int i = 0; i < enhancement.getWeight(); i++) finalList.Add(enhancement);
            }
        }
        if (finalList.Count == 0) return null;

        Enhancement finalEnhancement = null;
        int attempts = 50;

        finalList.RemoveAll(x => !x.isAvailable());
        if (finalList.Count == 0) return null;

        while (finalEnhancement == null)
        {
            int n = Random.Range(0, finalList.Count - 1);
            if(chosenEnhancements.Any(x => x.GetType() == finalList[n].GetType()) == false)
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
        Close();
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
        CurrentRerolls--;
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