using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Map : MonoBehaviour
{
    public static Map Instance;
    public List<MapTrack> tracks;
    public LayerMask nonPassableMask;
    public GameObject enemyPrefab;
    protected float SpawnRadius = 8f;

    public Transform startPosition;

    public GameObject gemPrefab;
    public GameObject bulletgemPrefab;
    public GameObject killEffectPrefab;
    public GameObject enemySpawnPrefab;
    //public GameObject bulletPrefab;
    public GameObject bulletSpawnPrefab;
    public GameObject coinPrefab;
    public GameObject bulletBasePrefab;
    public GameObject foodPrefab;
    public GameObject burningPrefab;

    //public GameObject bossAPrefab;
    public static float StageTime;
    protected float stagePartTime;

    public int beatsBeforeWave = 8;
    public int spawnRate = 3;

    public int beats = 0;

    private int waves = 0;

    public List<Wave> partAWaves;
    public List<Wave> partBWaves;
    public int CurrentDifficultyPoints = 0;
    public int part; // 0 = partA, 1 = BossA, 2 = partB, 3 = BossB

    public int WaveNumberOfEnemies;

    public string stageID;

    public List<Enemy> enemiesAlive;
    public List<Bullet> bulletsSpawned;
    public List<Drop> dropsSpawned;

    // NEW SPAWN SYSTEM
    public Vector2Int mapSize;
    public GameObject stageGrid;
    public Transform BossPosition, PlayerPositionOnBoss;
    protected List<GameObject> stageGridObjects;

    public GameObject bossGrid;
    public List<SpawnData> spawnPool;
    public List<StageTimeEvent> stageEvents;

    public static bool isBossWave = false;
    public GameObject mapObjects;
    public Boss currentBoss;
    public SpriteRenderer bossArea;

    private float spawnAngle;
    [SerializeField]protected Dialogue rabiEndDialogue;
    [SerializeField] public Animator CutsceneAnimator;

    public Fairy fairyCage;
    public static int global_enemy_spawn_modifier = 4;

    public static void ForceDespawnEnemies()
    {
        foreach (Enemy e in Instance.enemiesAlive)
        {
            e.ForceDespawn();
        }
        Instance.enemiesAlive.Clear();
    }

    public static void ForceDespawnBullets()
    {
        foreach (Bullet b in Instance.bulletsSpawned)
        {
            b.ForceDespawn();
        }
        Instance.bulletsSpawned.Clear();
    }

    public static void ForceDespawnDrops()
    {
        foreach (Drop b in Instance.dropsSpawned)
        {
            b.ForceDespawn();
        }
        Instance.dropsSpawned.Clear();
    }
    protected void Awake()
    {
        Instance = this;
        isBossWave = false;
        SetPools();
        BeatManager.SetTrack(tracks[0]);
        Player.ResetPosition();
        
        WaveNumberOfEnemies = 0;
        waves = 0;
        StageTime = 0;
        part = 0;
        beats = beatsBeforeWave - 1;
        WaveNumberOfEnemies = 0;
        bossArea.gameObject.SetActive(false);
        stageGridObjects = new List<GameObject> { stageGrid };
        for (int i = 0; i < 8; i++)
        {
            GameObject g = Instantiate(stageGrid);
            g.transform.parent = mapObjects.transform;
            stageGridObjects.Add(g);
        }

        Player.instance.transform.position = startPosition.position;
        Camera.main.transform.position = new Vector3(startPosition.position.x, startPosition.position.y, -60);
        StartMapEventListA();
    }

    protected virtual void StartMapEventListA()
    {
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-1");
        stageEvents = new List<StageTimeEvent>()
        {
            new AddEnemyEvent(EnemyType.TestEnemy, 1, 0, 0),
            new ChangeSpawnRateEvent(2, 0),
            new ChangeSpawnRateEvent(3, 20),
            new ChangeSpawnRateEvent(4, 40),
            new ChangeSpawnRateEvent(5, 60),
        };
    }
    public virtual void Start()
    {
        
    }
    
    public void SpawnBoss(SpawnData spawnData)
    {
        StartCoroutine(SpawnBossCoroutine(spawnData));
    }

    protected IEnumerator SpawnBossCoroutine(SpawnData spawnData)
    {
        // Despawn
        ForceDespawnEnemies();
        ForceDespawnBullets();
        ForceDespawnDrops();

        // Player can't move
        Player.instance.canDoAnything = false;
        PlayerCamera.instance.followPlayer = false;
        Player.instance.ForceDespawnAbilities(false);
        Player.instance.ResetAbilities();

        Player.instance.Exclamation.SetActive(true);
        AudioController.PlaySound(AudioController.instance.sounds.surpriseSfx);
        yield return new WaitForSeconds(1f);

        UIManager.Fade(false);
        yield return new WaitForSeconds(1f);
        mapObjects.SetActive(false);
        bossGrid.SetActive(true);
        Player.instance.transform.position = PlayerPositionOnBoss.position;
        PlayerCamera.instance.SetCameraPos(PlayerPositionOnBoss.position);

        Vector3 spawnPos = BossPosition.position; //new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

        UIManager.Fade(true);
        yield return new WaitForSeconds(1f);

        float time = 2;
        Vector3 c = PlayerCamera.instance.transform.position;
        while (time > 0)
        {
            c = Vector3.MoveTowards(c, new Vector3(spawnPos.x, spawnPos.y, -60), Time.deltaTime * 2f);
            PlayerCamera.instance.SetCameraPos(c);
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        time = 2;
        
        // Spawn boss with animation
        Boss enemy = (Boss)Enemy.GetEnemyOfType(spawnData.enemyType);
        currentBoss = enemy;
        enemy.AItype = spawnData.AItype;
        enemy.SpawnIndex = 0;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
        Player.instance.Exclamation.SetActive(false);
        // 2 seconds
        bossArea.gameObject.SetActive(true);
        bossArea.color = new Color(1, 1, 1, 0);
        bossArea.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        bossArea.transform.position = enemy.transform.position;
        while (bossArea.transform.localScale.x > 1.005f)
        {
            bossArea.color = new Color(1, 1, 1, Mathf.MoveTowards(bossArea.color.a, 0.8f, Time.deltaTime * 2.5f));
            bossArea.transform.localScale = Vector3.Lerp(bossArea.transform.localScale, Vector3.one, Time.deltaTime * 2.5f);
            yield return new WaitForEndOfFrame();
        }
        bossArea.color = new Color(1, 1, 1, 0.8f);
        bossArea.transform.localScale = Vector3.one;

        enemy.OnStart();
        yield break;
    }

    private SpawnData GetRandomEnemySpawn()
    {
        List<SpawnData> weightedList = new List<SpawnData>();

        foreach (var spawn in spawnPool)
        {
            for (int i = 0; i < spawn.weight; i++)
            {
                weightedList.Add(spawn);
            }
        }

        if (weightedList.Count == 0)
            return null;

        int randomIndex = Random.Range(0, weightedList.Count);
        return weightedList[randomIndex];
    }

    public static Enemy GetRandomEnemy()
    {
        if (Instance.enemiesAlive.Count == 0) return null;
        return Instance.enemiesAlive[Random.Range(0, Instance.enemiesAlive.Count - 1)];
    }

    public static Enemy GetRandomClosebyEnemy()
    {
        if (Instance.enemiesAlive.Count == 0) return null;
        Enemy e = null;
        int attempts = 40;

        Vector3 playerPos = Player.instance.transform.position;
        while (true)
        {
            if (attempts <= 0) break;
            attempts--;
            e = Instance.enemiesAlive[Random.Range(0, Instance.enemiesAlive.Count - 1)];
            if (Vector2.Distance(e.transform.position, playerPos) < 6) return e;
        }
        return null;
    }

    public virtual void SetPools()
    {
        PoolManager.CreatePool(typeof(Gem), gemPrefab, 30);
        PoolManager.CreatePool(typeof(BulletGem), bulletgemPrefab, 100);
        PoolManager.CreatePool(typeof(Coin), coinPrefab, 50);
        PoolManager.CreatePool(typeof(KillEffect), killEffectPrefab, 10);
        PoolManager.CreatePool(typeof(SpawnEffect), enemySpawnPrefab, 10);
        PoolManager.CreatePool(typeof(BulletBase), bulletBasePrefab, 500);
        PoolManager.CreatePool(typeof(BulletSpawnEffect), bulletSpawnPrefab, 100);
        PoolManager.CreatePool(typeof(Food), foodPrefab, 50);
        PoolManager.CreatePool(typeof(BurningVisualEffect), burningPrefab, 50);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused) spawnAngle += Time.deltaTime * 30;


        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            foreach (Enemy enemy in Instance.enemiesAlive)
            {
                enemy.OnBurn();
            }
        }
        HandleStageLoop();
        
        if (BeatManager.isGameBeat && BeatManager.isPlaying)
        {
            if (GameManager.isPaused) return;

            ReadEvents();
            beats++;
            if (beats >= beatsBeforeWave)
            {
                beats = 0;
                for (int i = 0; i < spawnRate + global_enemy_spawn_modifier; i++)
                {
                    SpawnEnemy(GetRandomEnemySpawn());
                }
            }
        }
        
        StageTime += Time.deltaTime;
        stagePartTime += Time.deltaTime;

        
        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            StageTime += 30;
            stagePartTime += 30;
        }
        
    }

    private void ReadEvents()
    {
        List<StageTimeEvent> events = stageEvents.FindAll(x => x.time < stagePartTime);

        foreach (StageTimeEvent e in events)
        {
            e.Trigger();
            Debug.Log($"Removing event {e.GetType()}");
            stageEvents.Remove(e);
        }
    }

    protected virtual void HandleStageLoop()
    {
        int loopX = Mathf.FloorToInt(Player.instance.transform.position.x / mapSize.x);
        int loopY = Mathf.FloorToInt(Player.instance.transform.position.y / mapSize.y);

        // Calculate new grid position based on loop index
        float gridX = loopX * mapSize.x;
        float gridY = loopY * mapSize.y;
        stageGridObjects[0].transform.position = new Vector2(gridX, gridY);
        stageGridObjects[1].transform.position = new Vector2(gridX - mapSize.x, gridY - mapSize.y);
        stageGridObjects[2].transform.position = new Vector2(gridX, gridY - mapSize.y);
        stageGridObjects[3].transform.position = new Vector2(gridX + mapSize.x, gridY - mapSize.y);

        stageGridObjects[4].transform.position = new Vector2(gridX - mapSize.x, gridY);
        stageGridObjects[5].transform.position = new Vector2(gridX + mapSize.x, gridY);

        stageGridObjects[6].transform.position = new Vector2(gridX - mapSize.x, gridY + mapSize.y);
        stageGridObjects[7].transform.position = new Vector2(gridX, gridY + mapSize.y);
        stageGridObjects[8].transform.position = new Vector2(gridX + mapSize.x, gridY + mapSize.y);

    }

    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawLine(Vector3.zero, new Vector3(mapSize.x, 0, 0));
        Gizmos.DrawLine(Vector3.zero, new Vector3(0, mapSize.y, 0));
        Gizmos.DrawLine(new Vector3(mapSize.x, 0, 0), new Vector3(mapSize.x, mapSize.y, 0));
        Gizmos.DrawLine(new Vector3(0, mapSize.y, 0), new Vector3(mapSize.x, mapSize.y, 0));
    }

    public static void StopMap()
    {
        Instance.OnStopMap();
        Instance.RemoveAllPools();
        Instance.StopAllCoroutines();
    }

    public virtual void OnStopMap() { }

    public virtual void RemoveAllPools()
    {
        PoolManager.RemovePool(typeof(Gem));
        PoolManager.RemovePool(typeof(BulletGem));
        PoolManager.RemovePool(typeof(Coin));
        PoolManager.RemovePool(typeof(KillEffect));
        PoolManager.RemovePool(typeof(SpawnEffect));
        PoolManager.RemovePool(typeof(BulletBase));
        PoolManager.RemovePool(typeof(BulletSpawnEffect));
        PoolManager.RemovePool(typeof(Food));
        PoolManager.RemovePool(typeof(BurningVisualEffect));
    }

    public List<int> spawnDirections = new List<int>();

    public static void SpawnEnemy(SpawnData spawnData)
    {
        if (spawnData == null) return;

        float x, y;
        if (Instance.spawnDirections.Count == 0)
        {
            Instance.spawnDirections = new List<int> { 0,1,2,3 };
        }
        Vector3 spawnPos = Vector3.zero;
        int spawnDir = Instance.spawnDirections[Random.Range(0, Instance.spawnDirections.Count - 1)];

        Debug.Log(spawnDir);
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        switch (spawnDir)
        {
            default:
            case 0: // Right
                spawnPos = new Vector2(cam.transform.position.x + (camWidth / 2) + Random.Range(0, 3), Random.Range(cam.transform.position.y - (camHeight / 2), cam.transform.position.y + (camHeight / 2)));
                break;
            case 1: // Top
                spawnPos = new Vector2(Random.Range(cam.transform.position.x - (camWidth / 2), cam.transform.position.x + (camWidth / 2)), cam.transform.position.y + (camHeight / 2) + Random.Range(0, 3));
                break;
            case 2: // Left
                spawnPos = new Vector2(cam.transform.position.x - (camWidth / 2) - Random.Range(0, 3), Random.Range(cam.transform.position.y - (camHeight / 2), cam.transform.position.y + (camHeight / 2)));
                break;
            case 3: // Bottom
                spawnPos = new Vector2(Random.Range(cam.transform.position.x - (camWidth / 2), cam.transform.position.x + (camWidth / 2)), cam.transform.position.y - (camHeight / 2) - Random.Range(0, 3));
                break;
        }

        Enemy enemy = Enemy.GetEnemyOfType(spawnData.enemyType);
        enemy.AItype = spawnData.AItype;
        enemy.SpawnIndex = 0;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
        Instance.spawnDirections.Remove(spawnDir);
    }

    public static void SpawnElite(SpawnData spawnData)
    {
        Instance.StartCoroutine(Instance.SpawnEliteCoroutine(spawnData));
    }

    private IEnumerator SpawnEliteCoroutine(SpawnData spawnData)
    {
        if (spawnData == null) yield break;

        AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
        SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();
        float angle, x, y;
        while (true)
        {
            angle = Random.Range(0, 360f);
            x = Player.instance.transform.position.x + (3 * Mathf.Cos(angle));
            y = Player.instance.transform.position.y + (3 * Mathf.Sin(angle));
            if (!isWallAt(new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y)))) break;
        }
        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        spawnEffect.transform.position = spawnPos;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);
        AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);

        Enemy enemy = Enemy.GetEnemyOfType(spawnData.enemyType);
        enemy.AItype = spawnData.AItype;
        enemy.SpawnIndex = 0;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
    }

    public static void SpawnUniqueEnemy(SpawnData spawnData)
    {
        Instance.StartCoroutine(Instance.SpawnUniqueEnemyCoroutine(spawnData));
    }

    private IEnumerator SpawnUniqueEnemyCoroutine(SpawnData spawnData)
    {
        if (spawnData == null) yield break;

        float angle, x, y;
        while (true)
        {
            angle = Random.Range(0, 360f);
            x = Player.instance.transform.position.x + (Instance.SpawnRadius * Mathf.Cos(angle));
            y = Player.instance.transform.position.y + (Instance.SpawnRadius * Mathf.Sin(angle));
            if (!isWallAt(new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y)))) break;
        }
        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

        Enemy enemy = Enemy.GetEnemyOfType(spawnData.enemyType);
        enemy.AItype = spawnData.AItype;
        enemy.SpawnIndex = 0;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
    }

    public static IEnumerator SpawnEnemyAroundPlayer(SpawnData spawnData, int index)
    {
        
        SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();
        float angle, x, y;
        while (true)
        {
            angle = Random.Range(0, 360f);
            x = Player.instance.transform.position.x + (Instance.SpawnRadius * Mathf.Cos(angle));
            y = Player.instance.transform.position.y + (Instance.SpawnRadius * Mathf.Sin(angle));
            x = Mathf.Clamp(x, -19f, 18f);
            y = Mathf.Clamp(y, -11f, 10f);
            if (!isWallAt(new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y)))) break;
        }
        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        spawnEffect.transform.position = spawnPos;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);

        PoolManager.Return(spawnEffect.gameObject, typeof(SpawnEffect));

        Enemy enemy = Enemy.GetEnemyOfType(spawnData.enemyType);
        enemy.AItype = spawnData.AItype;
        enemy.SpawnIndex = index;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
        yield break;
    }

    public static IEnumerator SpawnEnemyAtPos(SpawnData spawnData, int index)
    {
        SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();
        
        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(spawnData.spawnPosition.x), Mathf.RoundToInt(spawnData.spawnPosition.y));
        spawnEffect.transform.position = spawnPos;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);

        PoolManager.Return(spawnEffect.gameObject, typeof(SpawnEffect));

        Enemy enemy = Enemy.GetEnemyOfType(spawnData.enemyType);
        enemy.AItype = spawnData.AItype;
        enemy.SpawnIndex = index;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
        yield break;
    }

    public static bool isWallAt(Vector2 position)
    {
        return Physics2D.OverlapBox(position, Vector2.one / 2f, 0, Instance.nonPassableMask);
    }

    public IEnumerator StartBossASequence()
    {
        UIManager.Instance.PlayerUI.UpdateBossBar(2000, 2000);
        UIManager.Instance.PlayerUI.SetBossBarName("TestBoss Name");
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stageboss")}");
        BeatManager.FadeOut(Time.deltaTime / 2f);
        yield return new WaitForSeconds(2f);

        SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();

        Vector3 spawnPos = Vector3.zero;
        spawnEffect.transform.position = spawnPos;

        AudioController.PlaySound(AudioController.instance.sounds.warningBossWaveSound);
        yield return new WaitForSeconds(1f);
        AudioController.PlaySound(AudioController.instance.sounds.warningBossWaveSound);
        yield return new WaitForSeconds(1f);
        AudioController.PlaySound(AudioController.instance.sounds.warningBossWaveSound);
        yield return new WaitForSeconds(1f);
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();

        PoolManager.Return(spawnEffect.gameObject, typeof(SpawnEffect));

        TestBoss boss = PoolManager.Get<TestBoss>();
        boss.transform.position = spawnPos;
        boss.OnSpawn();

        BeatManager.SetTrack(tracks[1]);
        BeatManager.StartTrack();
        yield break;
    }

    public IEnumerator StartBossBSequence()
    {
        UIManager.Instance.PlayerUI.UpdateBossBar(2000, 2000);
        UIManager.Instance.PlayerUI.SetBossBarName("TestBoss Name");
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stageboss")}-2");
        BeatManager.FadeOut(Time.deltaTime / 2f);
        yield return new WaitForSeconds(2f);

        SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();

        Vector3 spawnPos = Vector3.zero;
        spawnEffect.transform.position = spawnPos;

        AudioController.PlaySound(AudioController.instance.sounds.warningBossWaveSound);
        yield return new WaitForSeconds(1f);
        AudioController.PlaySound(AudioController.instance.sounds.warningBossWaveSound);
        yield return new WaitForSeconds(1f);
        AudioController.PlaySound(AudioController.instance.sounds.warningBossWaveSound);
        yield return new WaitForSeconds(1f);
        while (!BeatManager.isGameBeat) yield return new WaitForEndOfFrame();

        PoolManager.Return(spawnEffect.gameObject, typeof(SpawnEffect));

        TestBoss boss = PoolManager.Get<TestBoss>();
        boss.transform.position = spawnPos;
        boss.OnSpawn();

        BeatManager.SetTrack(tracks[3]);
        BeatManager.StartTrack();
        yield break;
    }

    public void OnBossDeath(Boss boss)
    {
        isBossWave = false;
        StartCoroutine(BossDefeatCoroutine(boss));
    }

    protected virtual IEnumerator BossDefeatCoroutine(Boss boss)
    {
        // This is supposed to be an animation
        ForceDespawnBullets();
        PlayerCamera.TriggerCameraShake(2f, 0.45f);
        Player.instance.isMoving = true;
        if (Player.instance is PlayerRabi) Player.instance.animator.Play("Rabi_Idle");
        yield return new WaitForSeconds(0.45f); // This is when the white screen is pure white
        Player.instance.canDoAnything = false;
        PlayerCamera.instance.followPlayer = false;
        BeatManager.FadeOut(2f);
        currentBoss = null;

        PoolManager.Return(boss.gameObject, boss.GetType());
        Player.instance.facingRight = true;
        Player.instance.Sprite.transform.localScale = Vector3.one;
        PlayerCamera.instance.SetOnPlayer();

        foreach (GameObject g in stageGridObjects)
        {
            g.SetActive(false);
        }
        bossGrid.SetActive(false);
        Dialogue dialogue = Player.instance is PlayerRabi ? rabiEndDialogue : rabiEndDialogue;
        UIManager.Instance.dialogueMenu.StartCutscene(dialogue.entries);
        while (!UIManager.Instance.dialogueMenu.hasFinished) yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);

        UIManager.Instance.StageFinish.SetActive(true);
        AudioController.PlayMusic(AudioController.instance.sounds.stageComplete, false);
        
        yield return new WaitForSeconds(6);

        float goalPos = Player.instance.transform.position.x + 12;
        if (Player.instance is PlayerRabi) Player.instance.animator.Play("Rabi_Move");
        while (Player.instance.transform.position.x < goalPos)
        {
            Player.instance.transform.position += (Vector3.right * 4f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        GameManager.runData.stageMulti++;
        GameManager.LoadNextStage("Stage1-2");
        yield break;
    }

    public virtual IEnumerator BossADeathCoroutine()
    {
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-2");
        BeatManager.FadeOut(Time.deltaTime / 2f);
        yield return new WaitForSeconds(2f);

        BeatManager.SetTrack(tracks[2]);
        BeatManager.StartTrack();
        part = 2;
        waves++;

        Debug.Log(waves);
        beats = 0;
        yield break;
    }

    public virtual IEnumerator VictoryCoroutine()
    {
        BeatManager.FadeOut(Time.deltaTime / 2f);
        yield return new WaitForSeconds(2f);

        // VICTORY MENU BEHAVIOUR HERE
        yield break;
    }
}
