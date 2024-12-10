using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] TextMeshProUGUI endText;

    [SerializeField] TextMeshProUGUI retryText, mainmenuText;
    public void OnRetryClick()
    {
        AudioController.FadeOut();
        StartCoroutine(RetryCoroutine());
    }

    public void OnMainMenuClick()
    {

    }

    private IEnumerator RetryCoroutine()
    {
        yield return CloseCoroutine();
        Time.timeScale = 1.0f;

        UIManager.Fade(false);
        yield return new WaitForSeconds(1);
        UIManager.Instance.SetGameOverBG(false);
        UIManager.Instance.PlayerUI.HideBossBar();

        Player.instance.ForceDespawnAbilities(false);
        Player.instance.ResetAbilities();
        Player.instance.Despawn();
        UIManager.Instance.PlayerUI.OnReset();
        GameManager.LoadPlayer(GameManager.runData.characterPrefab);
        GameManager.LoadMap("Stage1a");
    }

    public void Open()
    {
        UIManager.Instance.PlayerUI.OnOpenMenu();
        retryText.text = "Retry"; //Localization.GetLocalizedString("gameover.retry");
        mainmenuText.text = "Main Menu"; //Localization.GetLocalizedString("gameover.mainmenu");
        endText.text = $"{Localization.GetLocalizedString("gameover.time")}{UIManager.Instance.PlayerUI.GetStageTime()}\n{Localization.GetLocalizedString("gameover.enemiesdefeated")}12\n\n{Localization.GetLocalizedString("gameover.laststage")}{UIManager.Instance.PlayerUI.GetStageName()}";
        StartCoroutine(OpenCoroutine());
    }

    public void Close()
    {
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
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    IEnumerator CloseCoroutine()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.unscaledDeltaTime * 2f);
            yield return new WaitForEndOfFrame();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
