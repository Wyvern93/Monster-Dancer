using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayerUI PlayerUI;
    [SerializeField] Image fade;
    private bool fadeIn;
    private float fadeAlpha = 1f;

    [SerializeField] Image gameOverBG;
    [SerializeField] GameOverMenu gameOverMenu;
    [SerializeField] AudioCalibrationMenu calibrationMenu;
    [SerializeField] public GameObject StageFinish;
    public DialogueMenu dialogueMenu;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetGameOverBG(bool visible)
    {
        gameOverBG.color = new Color(0, 0, 0, visible ? 1 : 0);
    }

    public void StartGameOverScreen()
    {
        gameOverMenu.Open();
    }

    public void OpenCalibrationMenu()
    {
        calibrationMenu.Open();
    }

    public void CloseCalibrationMenu() 
    {
        calibrationMenu.Close();
    }

    public static void Fade(bool fadein)
    {
        Instance.fadeIn = fadein;
    }

    // Update is called once per frame
    void Update()
    {
        fadeAlpha = Mathf.MoveTowards(fadeAlpha, fadeIn ? 0 : 1f, Time.deltaTime * 8f);
        fade.color = new Color(0, 0, 0, fadeAlpha);
    }
}
