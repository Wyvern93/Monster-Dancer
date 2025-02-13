using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class AudioCalibrationMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI calibrationNumber;

    [SerializeField] TextMeshProUGUI continueText;

    [SerializeField] MapTrack calibrationTrack;
    public void OnContinueClick()
    {
        AudioController.FadeOut();
        StartCoroutine(ContinueCoroutine());
    }

    private IEnumerator ContinueCoroutine()
    {
        yield return CloseCoroutine();

        SaveManager.PersistentSaveData.SetData("settings.audio_offset", BeatManager.audio_offset);
        SaveManager.SaveFile(SaveManager.PersistentSaveData);
        UIManager.Fade(false);

        GameManager.LoadPlayer(GameManager.runData.characterPrefab);
        GameManager.LoadMap("Stage1a");
    }

    public void Open()
    {
        UIManager.Fade(true);
        BeatManager.SetTrack(calibrationTrack);
        BeatManager.StartTrack();
        continueText.text = "Continue"; //Localization.GetLocalizedString("audiocalibration.continue");
        titleText.text = "Audio Calibration"; // Localization.GetLocalizedString("audiocalibration.title");
        text.text = "Use the arrow keys to change the audio offset and sync it with the indicator"; //Localization.GetLocalizedString("audiocalibration.text");
        calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
        StartCoroutine(OpenCoroutine());
    }

    public void Close()
    {
        UIManager.Fade(false);
        BeatManager.Stop();
        StartCoroutine(CloseCoroutine());
    }

    public void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    IEnumerator OpenCoroutine()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.unscaledDeltaTime * 2f);
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    IEnumerator CloseCoroutine()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        UIManager.Instance.PlayerUI.HideUI();
        UIManager.Fade(false);
        yield return new WaitForSeconds(0.5f);
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.unscaledDeltaTime * 2f);
            yield return null;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            AudioController.PlaySound(AudioController.instance.sounds.bossPhaseEnd, side: true);
        }
        if (canvasGroup.alpha == 0) return;
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
            {
                BeatManager.audio_offset -= 0.01f;
                calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
            }
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
            {
                BeatManager.audio_offset += 0.01f;
                calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
            {
                BeatManager.audio_offset -= 0.1f;
                calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
            }
            if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
            {
                BeatManager.audio_offset += 0.1f;
                calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
            }
        }
        else
        {
            if (Gamepad.current.leftStick.left.wasPressedThisFrame)
            {
                BeatManager.audio_offset -= 0.01f;
                calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
            }

            if (Gamepad.current.leftStick.right.wasPressedThisFrame)
            {
                BeatManager.audio_offset += 0.01f;
                calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
            }

            if (Gamepad.current.leftStick.up.wasPressedThisFrame)
            {
                BeatManager.audio_offset -= 0.1f;
                calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
            }
            if (Gamepad.current.leftStick.down.wasPressedThisFrame)
            {
                BeatManager.audio_offset += 0.1f;
                calibrationNumber.text = (Mathf.RoundToInt(BeatManager.audio_offset * 100f)).ToString();
            }

        }
        
    }
}
