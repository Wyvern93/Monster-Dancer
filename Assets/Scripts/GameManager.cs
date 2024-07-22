using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static string currentScene = "";
    public static RunData runData;
    public static bool isPaused;

    [SerializeField] GameObject selectedCharacter;
    public static bool isLoading;

    public List<Enhancement> EnhancementList;
    public static bool compassless;
    public IconList iconList;
    public GameObject spriteTrailPrefab;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        UIManager.Instance.PlayerUI.HideUI();
        UIManager.Instance.SetGameOverBG(false);

        SaveManager.LoadFiles();
        Localization.LoadLanguage(SaveManager.PersistentSaveData.GetData<string>("settings.language"));
        IconList.instance = iconList;
    }

    public static void SetSettings()
    {
        List<Resolution> resolutions = Screen.resolutions.ToList();
        Resolution res = resolutions[SaveManager.PersistentSaveData.GetData<int>("settings.resolution")];
        Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed, res.refreshRateRatio);

        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.fullScreen = SaveManager.PersistentSaveData.GetData<bool>("settings.fullscreen");

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = SaveManager.PersistentSaveData.GetData<int>("settings.maxFps");

        AudioController.instance.SetMaxVolume();
        BeatManager.audio_offset = SaveManager.PersistentSaveData.GetData<float>("settings.audio_offset");
    }

    void Start()
    {
        isLoading = true;

        SetSettings();

        runData = new RunData();
        runData.currentLoop = 1;
        runData.currentMap = "SampleScene";
        runData.characterPrefab = selectedCharacter;
        runData.isInfinite = false;

        PoolManager.CreatePool(typeof(SpriteRenderer), spriteTrailPrefab, 100);
        
        //LoadPlayer(runData.characterPrefab);
        //LoadMap(runData.currentMap);
        UIManager.Instance.OpenCalibrationMenu();
    }


    void Update()
    {
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            Player.instance.Despawn();
            LoadPlayer(runData.characterPrefab);
            LoadMap("SampleScene");
        }
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            compassless = !compassless;
        }
    }

    public static void LoadMap(string map)
    {
        instance.StartCoroutine(instance.LoadMapCoroutine(map));
    }

    public static void LoadPlayer(GameObject character)
    {
        GameObject playerObj = Instantiate(character);
        playerObj.name = "Player";
        instance.LoadDefaultAugments();
        UIManager.Instance.PlayerUI.HideSPBar();
    }

    public void LoadDefaultAugments()
    {
        runData.possibleStatEnhancements = new List<Enhancement>()
        { new StatDMGEnhancement(), new StatHPEnhancement(), new StatEXPPercentEnhancement(), new StatCritChanceEnhancement() };
    }

    private IEnumerator LoadMapCoroutine(string map)
    {
        isLoading = true;
        UIManager.Instance.PlayerUI.HideUI();
        UIManager.Fade(false);
        yield return new WaitForSeconds(0.5f); //UIManager.WaitForFade();
        if (Map.Instance) Map.StopMap();

        if (currentScene != "")
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(currentScene);
            while (!unload.isDone) yield return new WaitForEndOfFrame();
        }
        AsyncOperation load = SceneManager.LoadSceneAsync(map, LoadSceneMode.Additive);
        while (!load.isDone) yield return new WaitForEndOfFrame();
        UIManager.Fade(true);
        UIManager.Instance.PlayerUI.CreatePools();
        currentScene = map;
        BeatManager.StartTrack();
        UIManager.Instance.PlayerUI.ShowUI();
        isLoading = false;
        yield break;
    }
}
