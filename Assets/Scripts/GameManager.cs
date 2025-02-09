using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

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
    public static bool isDebugMode = true;
    public static bool infiniteHP = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        iconList.abilityAtlas = Resources.LoadAll<Sprite>("UI/AbilityList");
        iconList.reloadAtlas = Resources.LoadAll<Sprite>("UI/reloadIcons");
        iconList.itemAtlas = Resources.LoadAll<Sprite>("UI/ItemList");
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
        runData.stageMulti = 0;
        runData.currentMap = "Stage1a";
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
            LoadMap("Stage1a");
        }
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            BeatManager.compassless = !BeatManager.compassless;
            BeatManager.UpdatePulseAnimator();
        }

        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            infiniteHP = !infiniteHP;
        }
    }

    public static void LoadMap(string map)
    {
        instance.StartCoroutine(instance.LoadMapCoroutine(map));
    }

    public static void LoadNextStage(string map)
    {
        instance.StartCoroutine(instance.LoadNextStageCoroutine(map));
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
        { new StatDMGEnhancement(), new StatHPEnhancement(), new StatEXPPercentEnhancement(), new StatCritChanceEnhancement(), new MovSpeedEnhancement() };
        runData.possibleItemEnhancements = new List<Enhancement>()
        {
            new BlessedFigureItemEnhancement(),
            new SpindiscItemEnhancement(),
            new FireworksKitItemEnhancement(),
            new DetonationCatalystItemEnhancement(),
            new HotSauceBottleItemEnhancement(),
            new CursedNecklaceItemEnhancement(),
            new CatEyeMedalionItemEnhancement(),
            new TrainingCertificateItemEnhancement()
        };
    }

    private IEnumerator LoadMapCoroutine(string map)
    {
        isLoading = true;
        UIManager.Instance.PlayerUI.HideUI();
        UIManager.Fade(false);
        yield return new WaitForSeconds(0.5f); //UIManager.WaitForFade();
        if (Stage.Instance) Stage.StopMap();

        if (currentScene != "")
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(currentScene);
            while (!unload.isDone) yield return null;
        }
        AsyncOperation load = SceneManager.LoadSceneAsync(map, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;
        UIManager.Fade(true);
        UIManager.Instance.PlayerUI.CreatePools();
        currentScene = map;
        BeatManager.StartTrack();
        UIManager.Instance.PlayerUI.ShowUI();
        isLoading = false;
        yield break;
    }

    private IEnumerator LoadNextStageCoroutine(string map)
    {
        isLoading = true;
        UIManager.Instance.PlayerUI.HideUI();
        UIManager.Fade(false);
        yield return new WaitForSeconds(0.5f); //UIManager.WaitForFade();
        if (Stage.Instance) Stage.StopMap();

        if (currentScene != "")
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(currentScene);
            while (!unload.isDone) yield return null;
        }
        AsyncOperation load = SceneManager.LoadSceneAsync(map, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;
        UIManager.Fade(true);
        UIManager.Instance.PlayerUI.CreatePools();
        currentScene = map;
        BeatManager.StartTrack();
        UIManager.Instance.PlayerUI.ShowUI();
        isLoading = false;
        Player.ResetPosition();
        Player.instance.transform.position = Stage.Instance.startingStagePoint.transform.position;
        Camera.main.transform.position = new Vector3(Stage.Instance.startingStagePoint.transform.position.x, Stage.Instance.startingStagePoint.transform.position.y, Camera.main.transform.position.z);
        Player.instance.canDoAnything = true;

        PlayerCamera.instance.SetOnPlayer();
        PlayerCamera.instance.followPlayer = true;
        Player.instance.isMoving = false;
        if (Player.instance is PlayerRabi) Player.instance.animator.Play("Rabi_Idle");
        yield break;
    }
}
