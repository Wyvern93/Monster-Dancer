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

    public List<DialogueEntry> entries;
    private int index = 0;

    public bool isOpen;
    public bool hasFinished;
    private bool isWriting;

    public void Awake()
    {
        group.alpha = 0;
    }

    public void Open(List<DialogueEntry> dialogue)
    {
        hasFinished = false;
        entries = dialogue;
        index = 0;
        dialogueText.text = string.Empty;
        if (dialogue[0].leftSide)
        {
            leftNameObj.SetActive(true);
            rightNameObj.SetActive(false);
            
            leftPortraitSpr.color = Color.white;
            
            if (entries[0].rightPortrait == "NoPortrait") rightPortraitSpr.color = Color.clear;
            else rightPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            leftPortrait.Play(dialogue[0].leftPortrait);
            rightPortrait.Play(dialogue[0].rightPortrait);
        }
        else
        {
            leftNameObj.SetActive(false);
            rightNameObj.SetActive(true);
   
            leftPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            
            if (entries[0].leftPortrait == "NoPortrait") leftPortraitSpr.color = Color.clear;
            else rightPortraitSpr.color = Color.white;
            leftPortrait.Play(dialogue[0].leftPortrait);
            rightPortrait.Play(dialogue[0].rightPortrait);
        }

        StartCoroutine(DialogueOpen());
    }

    public void Close()
    {
        StartCoroutine(DialogueClose());
    }

    IEnumerator DialogueOpen()
    {
        StartCoroutine(DisplayDialogue(index));
        yield return new WaitForEndOfFrame();
        while (group.alpha < 1)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, 1, Time.deltaTime * 4f);
            yield return new WaitForEndOfFrame();
        }
        isOpen = true;
        
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

    IEnumerator DisplayDialogue(int entry)
    {
        isWriting = true;
        string dialogue = entries[entry].text;
        string currentDialogue = "";

        if (entries[entry].leftSide)
        {
            leftNameObj.SetActive(true);
            rightNameObj.SetActive(false);
            leftNameText.text = entries[entry].name;
            rightNameText.text = string.Empty;
            leftPortrait.Play(entries[index].leftPortrait);
           
            rightPortrait.Play(entries[index].rightPortrait);
            leftPortraitSpr.color = Color.white;
            rightPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            if (entries[index].rightPortrait == "NoPortrait") rightPortraitSpr.color = Color.clear;
        }
        else
        {
            leftNameObj.SetActive(false);
            rightNameObj.SetActive(true);
            leftNameText.text = string.Empty;
            rightNameText.text = entries[entry].name;
            leftPortrait.Play(entries[index].leftPortrait);
            rightPortrait.Play(entries[index].rightPortrait);
            leftPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            rightPortraitSpr.color = Color.white;
            if (entries[index].leftPortrait == "NoPortrait") leftPortraitSpr.color = Color.clear;
        }

        string[] words = dialogue.Split(' ');

        dialogueText.maxVisibleCharacters = 0;

        dialogueText.text = dialogue;
        for (int i = 0; i < dialogue.Length; i++)
        {
            dialogueText.maxVisibleCharacters++;
            if (i % 3 == 0) AudioController.PlaySound(AudioController.instance.sounds.ui_dialogue_char, Random.Range(0.8f, 1.2f));
            yield return new WaitForEndOfFrame();
        }
        dialogueText.maxVisibleCharacters = dialogue.Length;
        isWriting = false;
    }

    public void Update()
    {
        if (!isOpen) return;
        if (isWriting)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                StopAllCoroutines();
                isWriting = false;
                dialogueText.text = entries[index].text;
                dialogueText.maxVisibleCharacters = entries[index].text.Length;
            }
        }
        else
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                index++;
                if (index >= entries.Count) Close();
                else
                {
                    StartCoroutine(DisplayDialogue(index));
                }
            }
        }
    }
}