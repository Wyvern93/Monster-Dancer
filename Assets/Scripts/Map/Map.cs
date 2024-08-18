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
    protected float SpawnRadius = 15f;

    public Transform startPosition;

    public GameObject gemPrefab;
    public GameObject bulletgemPrefab;
    public GameObject killEffectPrefab;
    public GameObject enemySpawnPrefab;
    //public GameObject bulletPrefab;
    public GameObject bulletSpawnPrefab;
    public GameObject coinPrefab;

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
    [SerializeField] GameObject stageGrid;
    protected List<GameObject> stageGridObjects;
    public List<SpawnData> spawnPool;
    public List<StageTimeEvent> stageEvents;

    public static bool isBossWave = false;
    public GameObject mapObjects;
    public Boss currentBoss;
    public SpriteRenderer bossArea;

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
        // Fade off music
        BeatManager.FadeOut(1);
        yield return new WaitForSeconds(2);
        // Player can't move
        Player.instance.canDoAnything = false;
        // Move camera target to where boss is going to spawn
        float angle, x, y;
        while (true)
        {
            angle = Random.Range(0, 360f);
            x = Player.instance.transform.position.x + (3 * Mathf.Cos(angle));
            y = Player.instance.transform.position.y + (3 * Mathf.Sin(angle));
            if (!isWallAt(new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y)))) break;
        }
        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

        float time = 2;
        while (time > 0)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, new Vector3(spawnPos.x, spawnPos.y, Camera.main.transform.position.z), Time.deltaTime * 2f);
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

        // 2 seconds
        bossArea.gameObject.SetActive(true);
        bossArea.color = new Color(1, 1, 1, 0);
        bossArea.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        bossArea.transform.position = enemy.transform.position;
        while (bossArea.transform.localScale.x > 1.02f)
        {
            bossArea.color = new Color(1, 1, 1, Mathf.MoveTowards(bossArea.color.a, 0.8f, Time.deltaTime / 2f));
            bossArea.transform.localScale = Vector3.Lerp(bossArea.transform.localScale, Vector3.one, Time.deltaTime / 2f);
            yield return new WaitForEndOfFrame();
        }
        bossArea.transform.localScale = Vector3.one;

        Vector3 target = (new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, Camera.main.transform.position.z) + enemy.transform.position) / 2f;
        target.z = -60;
        while (time > 0)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, target, Time.deltaTime * 2f);
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Player.instance.SetCameraPos(target);

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

    private List<Wave> ShuffleWaves(List<Wave> list)
    {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
        return list;
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
        PoolManager.CreatePool(typeof(TestEnemy), enemyPrefab, 10);
        //PoolManager.CreatePool(typeof(TestBoss), bossAPrefab, 1);
        PoolManager.CreatePool(typeof(KillEffect), killEffectPrefab, 10);
        PoolManager.CreatePool(typeof(SpawnEffect), enemySpawnPrefab, 10);

        //PoolManager.CreatePool(typeof(Bullet), bulletPrefab, 100);
        PoolManager.CreatePool(typeof(BulletSpawnEffect), bulletSpawnPrefab, 100);
    }

    // Update is called once per frame
    void Update()
    {
        HandleStageLoop();
        
        if (BeatManager.isGameBeat && BeatManager.isPlaying)
        {
            if (GameManager.isPaused) return;

            ReadEvents();
            beats++;
            if (beats >= beatsBeforeWave)
            {
                beats = 0;
                for (int i = 0; i < spawnRate; i++)
                {
                    SpawnEnemy(GetRandomEnemySpawn());
                }
            }
        }
        /*
        if (BeatManager.isGameBeat && BeatManager.isPlaying)
        {
            beats++;
            
            if (part == 0)
            {
                if (waves < 21 && (beats >= beatsBeforeWave || enemiesAlive.Count < WaveNumberOfEnemies * 0.2f || enemiesAlive.Count <= 0))
                {
                    beats = 0;
                    SpawnNextWave(partAWaves); // Normal A Wave
                }
                // Is next boss wave
                if (waves == 21 && enemiesAlive.Count == 0)
                {
                    beatsBeforeWave = 99999999;
                    StartCoroutine(StartBossASequence());
                    part = 1;
                }
            }

            if (part == 2)
            {
                if (waves < 42 && (beats >= beatsBeforeWave || enemiesAlive.Count < WaveNumberOfEnemies * 0.2f || enemiesAlive.Count <= 0))
                {
                    beats = 0;
                    SpawnNextWave(partBWaves); // Normal A Wave
                }
                if (waves == 42 && enemiesAlive.Count == 0)
                {
                    beatsBeforeWave = 99999999;
                    StartCoroutine(StartBossBSequence());
                    part = 3;
                }
            }
        }*/
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
        //PoolManager.RemovePool(typeof(TestEnemy));
        //PoolManager.RemovePool(typeof(TestBoss));
        PoolManager.RemovePool(typeof(KillEffect));
        PoolManager.RemovePool(typeof(SpawnEffect));

        //PoolManager.RemovePool(typeof(Bullet));
        PoolManager.RemovePool(typeof(BulletSpawnEffect));
    }

    public static void SpawnEnemy(SpawnData spawnData)
    {
        if (spawnData == null) return;

        float angle, x, y;
        while (true)
        {
            angle = Random.Range(0, 360f);
            x = Player.instance.transform.position.x + (Instance.SpawnRadius * Mathf.Cos(angle));
            y = Player.instance.transform.position.y + (Instance.SpawnRadius * Mathf.Sin(angle));
            if (!isWallAt(new Vector2(x, y))) break;
        }
        Vector3 spawnPos = new Vector3(x,y);

        Enemy enemy = Enemy.GetEnemyOfType(spawnData.enemyType);
        enemy.AItype = spawnData.AItype;
        enemy.SpawnIndex = 0;
        enemy.transform.position = spawnPos;
        enemy.OnSpawn();
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
        PoolManager.Return(spawnEffect.gameObject, typeof(SpawnEffect));

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

    private IEnumerator SpawnBullet()
    {
        BulletSpawnEffect spawnEffect = PoolManager.Get<BulletSpawnEffect>();
        float angle, x, y;
        while (true)
        {
            angle = Random.Range(0, 360f);
            x = Player.instance.transform.position.x + (SpawnRadius * Mathf.Cos(angle));
            y = Player.instance.transform.position.y + (SpawnRadius * Mathf.Sin(angle));
            if (!isWallAt(new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y)))) break;
        }
        spawnEffect.transform.position = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        yield return new WaitForSeconds(BeatManager.GetBeatDuration());

        PoolManager.Return(spawnEffect.gameObject, typeof(BulletSpawnEffect));

        Bullet bullet = PoolManager.Get<Bullet>();
        bullet.transform.position = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        bullet.OnSpawn();
        yield break;
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
        Player.instance.canDoAnything = false;
        if (Player.instance is PlayerRabi) Player.instance.animator.Play("Rabi_Idle");
        yield return new WaitForSeconds(0.45f); // This is when the white screen is pure white
        BeatManager.FadeOut(2f);
        currentBoss = null;

        PoolManager.Return(boss.gameObject, boss.GetType());
        Player.instance.facingRight = true;
        Player.instance.Sprite.transform.localScale = Vector3.one;
        Camera.main.transform.position = new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, Camera.main.transform.position.z);
        foreach (GameObject g in stageGridObjects)
        {
            g.SetActive(false);
        }

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
