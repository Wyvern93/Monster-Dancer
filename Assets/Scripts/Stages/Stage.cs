using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Stage : MonoBehaviour
{
    public static Stage Instance;
    public List<MapTrack> tracks;
    public LayerMask nonPassableMask;
    public LayerMask waterMask;
    public GameObject enemyPrefab;
    protected float SpawnRadius = 8f;

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
    public GameObject enemyGroupPrefab;

    //public GameObject bossAPrefab;
    public static float StageTime;
    protected float stagePartTime;

    public int beatsBeforeWave = 8;
    public int spawnRate = 3;

    public int beats = 0;

    [HideInInspector] public StageWave playingWave;
    public int currentWave = 0;
    public int patternNumber;
    public List<WavePreset> wavePresetsAvailable;

    public int CurrentDifficultyPoints = 0;

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
    public List<StageEvent> stageEvents;

    public static bool isBossWave = false;
    public GameObject mapObjects;
    public Boss currentBoss;
    public SpriteRenderer bossArea;

    private float spawnAngle;
    [SerializeField]protected Dialogue rabiEndDialogue;
    [SerializeField] public Animator CutsceneAnimator;

    public Fairy fairyCage;
    public static int global_enemy_spawn_modifier = 1;

    public List<StageEventType> possibleEventTypes;

    public List<StageWaveEnemyData> waveEnemyData;
    public int elitesDefeated;
    public bool nextWaveIsElite;
    public bool nextWaveIsBoss;
    public int currentOrbitalEvents;

    public float additionalRunners = 0;
    public float additionalBombers = 0;
    public float additionalShooters = 0;

    public StageCameraPoint startingStagePoint;
    public StageCameraPoint currentStagePoint;
    public static float remainingWaveTime;

    public bool showWaveTimer;

    public virtual void OnBossFightStart(Boss boss)
    {

    }

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
        playingWave = null;
        SetPools();
        BeatManager.SetTrack(tracks[0]);
        Player.ResetPosition();

        currentStagePoint = startingStagePoint;
        PlayerCamera.instance.SetCameraPos(currentStagePoint.transform.position);

        currentWave = 0;
        StageTime = 0;
        beats = beatsBeforeWave - 1;
        bossArea.gameObject.SetActive(false);
        stageGridObjects = new List<GameObject> { stageGrid };
        /*
        for (int i = 0; i < 8; i++)
        {
            GameObject g = Instantiate(stageGrid);
            g.transform.parent = mapObjects.transform;
            stageGridObjects.Add(g);
        }*/

        Player.instance.transform.position = startingStagePoint.transform.position;
        Camera.main.transform.position = new Vector3(startingStagePoint.transform.position.x, startingStagePoint.transform.position.y, -60);
        StartMapWaveList();
        UIManager.Instance.PlayerUI.OnCloseMenu();
    }

    protected virtual void StartMapWaveList()
    {
        //UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-1");
        /*
        stageEvents = new List<StageEvent>()
        {
            new AddEnemyEvent(EnemyType.TestEnemy, 1, 0, 0),
            new ChangeSpawnRateEvent(2, 0),
            new ChangeSpawnRateEvent(3, 20),
            new ChangeSpawnRateEvent(4, 40),
            new ChangeSpawnRateEvent(5, 60),
        };*/
        //foreach (StageWave wave in Instance.waves)
        //{
        //    wave.Initialize();
        //}
       
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

        //UIManager.Fade(false);
        //yield return new WaitForSeconds(1f);
        //mapObjects.SetActive(false);
        //bossGrid.SetActive(true);
        //Player.instance.transform.position = PlayerPositionOnBoss.position;
        //PlayerCamera.instance.SetCameraPos(PlayerPositionOnBoss.position);

        Vector3 spawnPos = BossPosition.position; 

        //UIManager.Fade(true);
        //yield return new WaitForSeconds(1f);

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
        enemy.aiType = spawnData.AItype;
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

    public static Enemy GetRandomClosebyEnemy(Vector2 basePos, float range = 6)
    {
        if (Instance.enemiesAlive.Count == 0) return null;
        Enemy e = null;
        int attempts = 40;

        while (true)
        {
            if (attempts <= 0) break;
            attempts--;
            e = Instance.enemiesAlive[Random.Range(0, Instance.enemiesAlive.Count - 1)];
            if (e.CurrentHP <= 0) continue;
            if (Vector2.Distance(e.transform.position, basePos) < range) return e;
        }
        return null;
    }

    public static Enemy GetClosestEnemyTo(Vector2 basePos, float range = 6)
    {
        if (Instance.enemiesAlive.Count == 0) return null;
        List<Enemy> closeEnemies = new List<Enemy>();
        Enemy e = null;
        int attempts = 40;

        while (true)
        {
            if (attempts <= 0) break;
            attempts--;
            e = Instance.enemiesAlive[Random.Range(0, Instance.enemiesAlive.Count - 1)];
            if (e.CurrentHP <= 0) continue;
            if (Vector2.Distance(e.transform.position, basePos) < range) closeEnemies.Add(e);
        }
        if (closeEnemies.Count == 0) return null;

        float dist = 999;
        foreach (Enemy enemy in closeEnemies)
        {
            float enemyDist = Vector2.Distance(enemy.transform.position, basePos);
            if (enemyDist < dist)
            {
                dist = enemyDist;
                e = enemy;
            }
        }
        return e;
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
        PoolManager.CreatePool(typeof(EnemyGroup), enemyGroupPrefab, 50);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isPaused) spawnAngle += Time.deltaTime * 30;

        HandleStageLoop();

        HandleStageMovement();
        
        if (BeatManager.isGameBeat && BeatManager.isPlaying)
        {
            if (GameManager.isPaused) return;

            //ReadEvents();
            beats++;
            /*
            if (beats >= beatsBeforeWave)
            {
                beats = 0;
                for (int i = 0; i < spawnRate + global_enemy_spawn_modifier; i++)
                {
                    EnemyGroup group = PoolManager.Get<EnemyGroup>();
                    group.aIType = EnemyAIType.Spread;
                    SpawnEnemy(GetRandomEnemySpawn(), group);
                    group.OnGroupInit();
                }
            }
            */
            TryToSpawnNextWave();
        }
        
        StageTime += Time.deltaTime;
        stagePartTime += Time.deltaTime;
        remainingWaveTime -= Time.deltaTime;

        
        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            ForceDespawnEnemies();
        }
        
    }

    protected void HandleStageMovement()
    {
        if (currentStagePoint == null) return;

        if (currentStagePoint.CanTrigger()) currentStagePoint.Trigger();
        if (currentStagePoint.CanMoveToNext()) currentStagePoint = currentStagePoint.next;
    }

    /*
    private void ReadEvents()
    {
        List<StageEvent> events = stageEvents.FindAll(x => x.time < stagePartTime);

        foreach (StageEvent e in events)
        {
            e.Trigger();
            Debug.Log($"Removing event {e.GetType()}");
            stageEvents.Remove(e);
        }
    }*/

    public void TryToSpawnNextWave()
    {
        if (!showWaveTimer && enemiesAlive.Count > 0) return; // Elites need to be defeated first
        else if (enemiesAlive.Count > 0 && remainingWaveTime > 0) return;
        //{
            //if (!enemiesAlive[0].isElite) return;
            //if (enemiesAlive.Count > 1) return;
        //}


        //Debug.Log("No enemies");
        if (playingWave == null) // First Wave
        {
            remainingWaveTime = 30;
            showWaveTimer = true;
            currentOrbitalEvents = 0;
            additionalRunners = 0;
            additionalBombers = 0;
            additionalShooters = 0;
            playingWave = new StageWave();
            playingWave.waveData = waveEnemyData[elitesDefeated];
            playingWave.Initialize();
            StartCoroutine(playingWave.Start());
            UIManager.Instance.PlayerUI.SetStageText($"Wave {currentWave + 1}");
            currentWave++;
            return;
        }

        if (!playingWave.isFinalized) return;

        UIManager.Instance.PlayerUI.SetStageText($"Wave {currentWave + 1}");
        if (nextWaveIsBoss)
        {
            remainingWaveTime = 30;
            showWaveTimer = false;
            currentOrbitalEvents = 0;
            additionalRunners = 0;
            additionalBombers = 0;
            additionalShooters = 0;
            playingWave = new StageWave();
            playingWave.waveData = waveEnemyData[elitesDefeated];
            //playingWave.choosenPreset = new WavePreset() { events = new List<StageEvent> { new SpawnBossEvent() { enemySpawnType = EnemySpawnType.BOSS } } };
            //playingWave.Initialize();
            SpawnBoss(new SpawnData() { enemyType = waveEnemyData[elitesDefeated].specialSpawnEnemy });
            playingWave.isInitialized = true;
            playingWave.choosenPreset = new WavePreset();
            playingWave.choosenPreset.events = new List<StageEvent>();
            nextWaveIsBoss = false;
            //StartCoroutine(playingWave.Start());
            playingWave.isFinalized = false;
            currentWave++;
        }
        else
        {
            currentOrbitalEvents = 0;
            playingWave = new StageWave();
            playingWave.waveData = waveEnemyData[elitesDefeated];
            if (nextWaveIsElite)
            {
                remainingWaveTime = 30;
                showWaveTimer = false;
                additionalRunners = 0;
                additionalBombers = 0;
                additionalShooters = 0;
                SpawnElite(new SpawnData() { enemyType = waveEnemyData[elitesDefeated].specialSpawnEnemy });
                playingWave.isInitialized = true;
                playingWave.choosenPreset = new WavePreset();
                playingWave.choosenPreset.events = new List<StageEvent>();
                nextWaveIsElite = false;
                //StartCoroutine(playingWave.Start());
                playingWave.isFinalized = false;
            }
            else
            {
                remainingWaveTime = 30;
                showWaveTimer = true;
                additionalRunners += 0.25f;
                additionalBombers += 0.135f;
                additionalShooters += 0.125f;
                playingWave.Initialize();
                StartCoroutine(playingWave.Start());
            }
            currentWave++;
        }
    }

    protected virtual void HandleStageLoop()
    {/*
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
        */
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
        ForceDespawnEnemies();
        ForceDespawnDrops();
        ForceDespawnBullets();
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
        PoolManager.RemovePool(typeof(EnemyGroup));
    }

    public List<int> spawnDirections = new List<int>();

    public static void SpawnSpreadEnemy(EnemyType enemyType, EnemyGroup group)
    {

        if (Instance.spawnDirections.Count == 0)
        {
            Instance.spawnDirections = new List<int> { 0,1,2,3 };
        }
        Vector3 spawnPos = Vector3.zero;
        int spawnDir = Instance.spawnDirections[Random.Range(0, Instance.spawnDirections.Count - 1)];

        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        switch (spawnDir)
        {
            default:
            case 0: // Right
                spawnPos = new Vector2(cam.transform.position.x + (camWidth / 2) + Random.Range(1, 4), Random.Range(cam.transform.position.y - (camHeight / 2), cam.transform.position.y + (camHeight / 2)));
                break;
            case 1: // Top
                spawnPos = new Vector2(Random.Range(cam.transform.position.x - (camWidth / 2), cam.transform.position.x + (camWidth / 2)), cam.transform.position.y + (camHeight / 2) + Random.Range(1, 4));
                break;
            case 2: // Left
                spawnPos = new Vector2(cam.transform.position.x - (camWidth / 2) - Random.Range(1, 4), Random.Range(cam.transform.position.y - (camHeight / 2), cam.transform.position.y + (camHeight / 2)));
                break;
            case 3: // Bottom
                spawnPos = new Vector2(Random.Range(cam.transform.position.x - (camWidth / 2), cam.transform.position.x + (camWidth / 2)), cam.transform.position.y - (camHeight / 2) - Random.Range(1, 4));
                break;
        }

        Enemy enemy = Enemy.GetEnemyOfType(enemyType);
        enemy.group = group;
        enemy.aiType = EnemyAIType.Spread;
        enemy.SpawnIndex = 0;
        enemy.transform.position = spawnPos;
        enemy.circleCollider.enabled = true;
        enemy.OnSpawn();
        Instance.spawnDirections.Remove(spawnDir);
    }

    public static void SpawnCircleHorde(EnemyType enemyType, EnemyGroup group, int number, bool chase)
    {
        if (Instance.spawnDirections.Count == 0)
        {
            Instance.spawnDirections = new List<int> { 0, 1, 2, 3 };
        }
        Vector3 spawnPos = Vector3.zero;
        int spawnDir = Instance.spawnDirections[Random.Range(0, Instance.spawnDirections.Count - 1)];

        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        switch (spawnDir)
        {
            default:
            case 0: // Right
                spawnPos = new Vector2(cam.transform.position.x + (camWidth / 2) + 4, Random.Range(cam.transform.position.y - (camHeight / 2), cam.transform.position.y + (camHeight / 2)));
                break;
            case 1: // Top
                spawnPos = new Vector2(Random.Range(cam.transform.position.x - (camWidth / 2), cam.transform.position.x + (camWidth / 2)), cam.transform.position.y + (camHeight / 2) + 4);
                break;
            case 2: // Left
                spawnPos = new Vector2(cam.transform.position.x - (camWidth / 2) - 4, Random.Range(cam.transform.position.y - (camHeight / 2), cam.transform.position.y + (camHeight / 2)));
                break;
            case 3: // Bottom
                spawnPos = new Vector2(Random.Range(cam.transform.position.x - (camWidth / 2), cam.transform.position.x + (camWidth / 2)), cam.transform.position.y - (camHeight / 2) - 4);
                break;
        }

        Vector3 playerPos = Player.instance.GetClosestPlayer(spawnPos) + (Vector3)UnityEngine.Random.insideUnitCircle * 2;
        Vector2 dir = playerPos - spawnPos;
        dir.Normalize();

        for (int i = 0; i < number; i++)
        {
            Vector3 random = spawnPos + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.05f); //(Vector3)(UnityEngine.Random.insideUnitCircle * size) + spawnPos;
            Enemy e = Enemy.GetEnemyOfType(enemyType);
            e.aiType = chase ? EnemyAIType.HordeChase : EnemyAIType.CircleHorde;
            e.transform.position = random;
            e.eventMove = dir;
            e.group = group;
            group.enemies.Add(e);
            e.OnSpawn();
            if (!chase) e.speed += 1.6f;
        }
    }

    public static void RespawnHorde(EnemyGroup enemyGroup)
    {
        if (Instance.spawnDirections.Count == 0)
        {
            Instance.spawnDirections = new List<int> { 0, 1, 2, 3 };
        }
        Vector3 spawnPos = Vector3.zero;
        int spawnDir = Instance.spawnDirections[Random.Range(0, Instance.spawnDirections.Count - 1)];

        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        switch (spawnDir)
        {
            default:
            case 0: // Right
                spawnPos = new Vector2(cam.transform.position.x + (camWidth / 2) + 4, Random.Range(cam.transform.position.y - (camHeight / 2), cam.transform.position.y + (camHeight / 2)));
                break;
            case 1: // Top
                spawnPos = new Vector2(Random.Range(cam.transform.position.x - (camWidth / 2), cam.transform.position.x + (camWidth / 2)), cam.transform.position.y + (camHeight / 2) + 4);
                break;
            case 2: // Left
                spawnPos = new Vector2(cam.transform.position.x - (camWidth / 2) - 4, Random.Range(cam.transform.position.y - (camHeight / 2), cam.transform.position.y + (camHeight / 2)));
                break;
            case 3: // Bottom
                spawnPos = new Vector2(Random.Range(cam.transform.position.x - (camWidth / 2), cam.transform.position.x + (camWidth / 2)), cam.transform.position.y - (camHeight / 2) - 4);
                break;
        }

        Vector3 playerPos = Player.instance.GetClosestPlayer(spawnPos) + (Vector3)UnityEngine.Random.insideUnitCircle * 2;
        Vector2 dir = playerPos - spawnPos;
        dir.Normalize();

        foreach (Enemy e in enemyGroup.enemies)
        {
            if (e.isDead || !e.gameObject.activeSelf) continue;

            Vector3 random = spawnPos + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.05f); //(Vector3)(UnityEngine.Random.insideUnitCircle * size) + spawnPos;
            e.transform.position = random;
            e.eventMove = dir;
        }
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
        spawnEffect.animator.Play("EliteSummon");

        Vector3 spawnPos = PlayerCamera.instance.transform.position;
        spawnPos.z = 0;
        spawnEffect.transform.position = spawnPos;
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);
        AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);

        Enemy enemy = Enemy.GetEnemyOfType(spawnData.enemyType);
        enemy.aiType = spawnData.AItype;
        enemy.SpawnIndex = 0;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
        nextWaveIsElite = false;
    }

    public static void SpawnUniqueEnemy(SpawnData spawnData, EnemyGroup group)
    {
        Instance.StartCoroutine(Instance.SpawnUniqueEnemyCoroutine(spawnData, group));
    }

    private IEnumerator SpawnUniqueEnemyCoroutine(SpawnData spawnData, EnemyGroup group)
    {
        if (spawnData == null) yield break;

        SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();
        spawnEffect.transform.position = spawnData.spawnPosition;
        spawnEffect.animator.Play("EnemySummon");
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);

        Enemy enemy = Enemy.GetEnemyOfType(spawnData.enemyType);
        enemy.aiType = group.aIType;
        enemy.SpawnIndex = 0;
        enemy.transform.position = spawnData.spawnPosition;
        enemy.group = group;
        enemy.OnSpawn();
    }

    public static IEnumerator SpawnEnemiesGeometric(EnemyType enemyType, EnemyGroup group, int number)
    {
        int orbitDistance = Instance.currentOrbitalEvents + 4;
        orbitDistance = Mathf.Clamp(orbitDistance, 2, 8);
        Vector3 spawnPos = PlayerCamera.instance.transform.position;
        spawnPos.z = 0;

        for (int i = 0; i < number; i++)
        {
            Vector3 enemyPos = spawnPos + ((Vector3)(BulletBase.angleToVector((360f / number) * i)) * orbitDistance);
            SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();
            spawnEffect.transform.position = enemyPos;
            spawnEffect.animator.Play("EnemySummon");
        }
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);

        group.totalEnemies = 0;
        for (int i = 0; i < number; i++)
        {
            Vector3 enemyPos = spawnPos + ((Vector3)(BulletBase.angleToVector((360f / number) * i)) * orbitDistance);
            Enemy enemy = Enemy.GetEnemyOfType(enemyType);
            enemy.aiType = group.aIType;
            enemy.SpawnIndex = i;
            enemy.transform.position = enemyPos;
            enemy.group = group;           
            group.enemies.Add(enemy);
            group.totalEnemies++;
            enemy.OnSpawn();
            enemy.speed += 1.6f;

        }
        group.orbitDistance = orbitDistance;
        Instance.currentOrbitalEvents++;
        group.OnGroupInit();
        yield break;
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
        enemy.aiType = spawnData.AItype;
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
        enemy.aiType = spawnData.AItype;
        enemy.SpawnIndex = index;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
        yield break;
    }

    public static bool isWallAt(Vector2 position)
    {
        return Physics2D.OverlapBox(position, Vector2.one / 2f, 0, Instance.nonPassableMask);
    }

    public static bool isWaterAt(Vector2 position)
    {
        return Physics2D.OverlapBox(position, Vector2.one / 2f, 0, Instance.waterMask);
    }

    public void OnBossDeath(Boss boss)
    {
        isBossWave = false;
        StartCoroutine(BossDefeatCoroutine(boss));
    }

    protected virtual IEnumerator BossDefeatCoroutine(Boss boss)
    {
        UIManager.Instance.PlayerUI.ShowBossBar(false);
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

    public virtual IEnumerator VictoryCoroutine()
    {
        BeatManager.FadeOut(Time.deltaTime / 2f);
        yield return new WaitForSeconds(2f);

        // VICTORY MENU BEHAVIOUR HERE
        yield break;
    }
}
