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

    [HideInInspector] public List<CutsceneEvent> entries;
    private CutsceneType cutsceneType;
    private int index = 0;

    public bool isOpen;
    private bool isOpening;
    public bool hasFinished;
    private bool isWriting;
    private bool moveToNext;

    public bool isInCutscene()
    {
        return !hasFinished;
    }

    public void Awake()
    {
        group.alpha = 0;
    }

    public void StartCutscene(CutsceneType cutsceneType)//(List<CutsceneEvent> entries)
    {
        Debug.Log("Start cutscene");
        this.cutsceneType = cutsceneType;
        Player.instance.SetVisible(false);
        if (Stage.Instance.currentBoss != null) Stage.Instance.currentBoss.SetVisible(false);

        Stage.Instance.actors.SetActive(true);
        UIManager.Instance.PlayerUI.OnOpenMenu();
        Stage.ForceDespawnEnemies();
        moveToNext = false;
        index = 0;
        if (cutsceneType == CutsceneType.Boss) entries = GetBossCutscene();
        else entries = GetEndStageCutscene();

        hasFinished = false;
        
        isOpening = false;
        isOpen = false;
        UIManager.Instance.PlayerUI.UpdateHealth();
    }

    private List<CutsceneEvent> GetBossCutscene()
    {
        if (Player.instance is PlayerRabi) return Stage.Instance.RabiBoss.entries;
        return null;
    }

    private List<CutsceneEvent> GetEndStageCutscene()
    {
        if (Player.instance is PlayerRabi) return Stage.Instance.RabiEnd.entries;
        return null;
    }

    public void StartDialogue(DialogueEntry entry)
    {
        hasFinished = false;
        dialogueText.text = string.Empty;
        /*
        if (entry.leftSide)
        {
            leftNameObj.SetActive(true);
            rightNameObj.SetActive(false);
            
            leftPortraitSpr.color = Color.white;

            if (entry.rightPortrait == null) rightPortraitSpr.color = Color.clear;
            else
            {
                rightPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                leftPortraitSpr.sprite = entry.leftPortrait;
                rightPortraitSpr.sprite = entry.rightPortrait;
            }
        }
        else
        {
            leftNameObj.SetActive(false);
            rightNameObj.SetActive(true);
   
            leftPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            if (entry.leftPortrait == null) leftPortraitSpr.color = Color.clear;
            else
            {
                rightPortraitSpr.color = Color.white;
                leftPortraitSpr.sprite = entry.leftPortrait;
                rightPortraitSpr.sprite = entry.rightPortrait;
            }
        }
        */
        StartCoroutine(DialogueOpen(entry));
    }

    public void Close()
    {
        StartCoroutine(DialogueClose());
    }

    IEnumerator DialogueOpen(DialogueEntry entry)
    {
        StartCoroutine(DisplayDialogue(entry));
        yield return null;
        while (group.alpha < 1)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, 1, Time.deltaTime * 4f);
            yield return null;
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
            yield return null;
        }
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

            if (entry.leftPortrait == null)
            {
                leftPortraitSpr.color = Color.clear;
            }
            else
            {
                leftPortraitSpr.sprite = entry.leftPortrait;
                leftPortraitSpr.color = Color.white;
            }

            if (entry.rightPortrait == null)
            {
                rightPortraitSpr.color = Color.clear;
            }
            else
            {
                rightPortraitSpr.sprite = entry.rightPortrait;
                rightPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            
        }
        else
        {
            leftNameObj.SetActive(false);
            rightNameObj.SetActive(true);
            leftNameText.text = string.Empty;
            rightNameText.text = entry.name;

            if (entry.leftPortrait == null)
            {
                leftPortraitSpr.color = Color.clear;
            }
            else
            {
                leftPortraitSpr.sprite = entry.leftPortrait;
                leftPortraitSpr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }

            if (entry.rightPortrait == null)
            {
                rightPortraitSpr.color = Color.clear;
            }
            else
            {
                rightPortraitSpr.sprite = entry.rightPortrait;
                rightPortraitSpr.color = Color.white;
            }

        }

        string[] words = dialogue.Split(' ');

        dialogueText.maxVisibleCharacters = 0;

        dialogueText.text = dialogue;
        for (int i = 0; i < dialogue.Length; i++)
        {
            yield return null;
            dialogueText.maxVisibleCharacters++;
            if (i % 3 == 0) AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.ui_dialogue_char, Random.Range(0.8f, 1.2f));
            yield return null;
        }
        dialogueText.maxVisibleCharacters = dialogue.Length;
        isWriting = false;
        yield break;
    }

    private void OnCutsceneEnd()
    {
        Debug.Log("On Cutscene end");
        hasFinished = true;
        if (cutsceneType == CutsceneType.Boss)
        {
            Player.instance.SetVisible(true);
            if (Stage.Instance.currentBoss != null) Stage.Instance.currentBoss.SetVisible(true);

            Stage.Instance.actors.SetActive(false);
        }
    }

    public void Update()
    {
        if (index >= entries.Count)
        {
            if (!hasFinished) OnCutsceneEnd();
            return;
        }

        if (entries[index].GetType() != typeof(DialogueEntry)) Close();

        // Read events
        if (entries[index].GetType() == typeof(PlayAnimationEvent))
        {
            Stage.Instance.CutsceneAnimator.Play((entries[index] as PlayAnimationEvent).animation);
            index++;
        }
        else if (entries[index].GetType() == typeof(ActorTeleportEvent))
        {
            ((ActorTeleportEvent)entries[index]).Trigger();
            index++;
        }
        else if (entries[index].GetType() == typeof(ActorFacingEvent))
        {
            ((ActorFacingEvent)entries[index]).Trigger();
            index++;
        }
        else if (entries[index].GetType() == typeof(ActorMoveEvent))
        {
            ((ActorMoveEvent)entries[index]).Trigger();
            index++;
        }
        else if (entries[index].GetType() == typeof(WaitForActorToEndEvent))
        {
            if (!((WaitForActorToEndEvent)entries[index]).HasFinished()) return;
            else index++;
        }
        else if (entries[index].GetType() == typeof(DialogueEntry))
        {
            if (!isOpen && !isOpening)
            {
                isOpening = true;
                StartDialogue(entries[index] as DialogueEntry);
                return;
            }
            else if (moveToNext)
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