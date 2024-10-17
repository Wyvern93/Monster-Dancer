using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] CanvasGroup group;
    [SerializeField] GameObject leftNameObj, rightNameObj;
    [SerializeField] TextMeshProUGUI leftNameText, rightNameText, dialogueText;
    [SerializeField] Animator leftPortrait, rightPortrait;
    [SerializeField] Image leftPortraitSpr, rightPortraitSpr;

    public List<CutsceneEvent> entries;
    private int index = 0;

    public bool isOpen;
    private bool isOpening;
    public bool hasFinished;
    private bool isWriting;
    private bool moveToNext;

    public void Awake()
    {
        group.alpha = 0;
    }

    public void StartCutscene(List<CutsceneEvent> entries)
    {
        moveToNext = false;
        index = 0;
        this.entries = entries;
        hasFinished = false;
        
        isOpening = false;
        isOpen = false;
        index = 0;
        
    }

    public void StartDialogue(DialogueEntry entry)
    {
        hasFinished = false;
        index = 0;
        dialogueText.text = string.Empty;
        if (entry.leftSide)
        {
            leftNameObj.SetActive(true);
            rightNameObj.SetActive(false);
            
            leftPortraitSpr.color = Color.white;
            
            if (entry.rightPortrait == "NoPortrait") rightPortraitSpr.color = Color.clear;
            else rightPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            leftPortrait.Play(entry.leftPortrait);
            rightPortrait.Play(entry.rightPortrait);
        }
        else
        {
            leftNameObj.SetActive(false);
            rightNameObj.SetActive(true);
   
            leftPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            
            if (entry.leftPortrait == "NoPortrait") leftPortraitSpr.color = Color.clear;
            else rightPortraitSpr.color = Color.white;
            leftPortrait.Play(entry.leftPortrait);
            rightPortrait.Play(entry.rightPortrait);
        }

        StartCoroutine(DialogueOpen(entry));
    }

    public void Close()
    {
        StartCoroutine(DialogueClose());
    }

    IEnumerator DialogueOpen(DialogueEntry entry)
    {
        StartCoroutine(DisplayDialogue(entry));
        yield return new WaitForEndOfFrame();
        while (group.alpha < 1)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, 1, Time.deltaTime * 4f);
            yield return new WaitForEndOfFrame();
        }
        isOpen = true;
        isOpening = false;

        yield break;
    }

    IEnumerator DialogueClose()
    {
        while (group.alpha > 0)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, 0, Time.deltaTime * 4f);
            yield return new WaitForEndOfFrame();
        }
        hasFinished = true;
        isOpen = false;
        yield break;
    }

    IEnumerator DisplayDialogue(DialogueEntry entry)
    {
        isWriting = true;
        string dialogue = entry.text;

        if (entry.leftSide)
        {
            leftNameObj.SetActive(true);
            rightNameObj.SetActive(false);
            leftNameText.text = entry.name;
            rightNameText.text = string.Empty;
            leftPortrait.Play(entry.leftPortrait);
           
            rightPortrait.Play(entry.rightPortrait);
            leftPortraitSpr.color = Color.white;
            rightPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            if (entry.rightPortrait == "NoPortrait") rightPortraitSpr.color = Color.clear;
        }
        else
        {
            leftNameObj.SetActive(false);
            rightNameObj.SetActive(true);
            leftNameText.text = string.Empty;
            rightNameText.text = entry.name;
            leftPortrait.Play(entry.leftPortrait);
            rightPortrait.Play(entry.rightPortrait);
            leftPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            rightPortraitSpr.color = Color.white;
            if (entry.leftPortrait == "NoPortrait") leftPortraitSpr.color = Color.clear;
        }

        string[] words = dialogue.Split(' ');

        dialogueText.maxVisibleCharacters = 0;

        dialogueText.text = dialogue;
        for (int i = 0; i < dialogue.Length; i++)
        {
            dialogueText.maxVisibleCharacters++;
            if (i % 3 == 0) AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.ui_dialogue_char, Random.Range(0.8f, 1.2f));
            yield return new WaitForEndOfFrame();
        }
        dialogueText.maxVisibleCharacters = dialogue.Length;
        isWriting = false;
        yield break;
    }

    public void Update()
    {
        if (index >= entries.Count) return;

        // Read events
        if (entries[index].GetType() == typeof(PlayAnimationEvent))
        {
            Map.Instance.CutsceneAnimator.Play((entries[index] as PlayAnimationEvent).animation);
            index++;
        }
        else if (entries[index].GetType() == typeof(DialogueEntry))
        {
            if (!isOpen && !isOpening)
            {
                isOpening = true;
                StartDialogue(entries[index] as DialogueEntry);
                return;
            }
            if (moveToNext)
            {
                moveToNext = false;
                StartCoroutine(DisplayDialogue(entries[index] as DialogueEntry));
            }
            DialogueEntry currentDialogue = entries[index] as DialogueEntry;
            if (isWriting)
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
                {
                    group.alpha = 1;
                    StopAllCoroutines();
                    isWriting = false;
                    dialogueText.text = currentDialogue.text;
                    dialogueText.maxVisibleCharacters = currentDialogue.text.Length;
                }
            }
            else
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
                {
                    index++;
                    if (index >= entries.Count) Close();
                    else moveToNext = true;
                }
            }
        }
    }
}