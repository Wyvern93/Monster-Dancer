using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance;
    public List<MapTrack> tracks;
    public LayerMask nonPassableMask;
    public GameObject enemyPrefab;
    float Radius = 6f;

    public GameObject gemPrefab;
    public GameObject bulletgemPrefab;
    public GameObject killEffectPrefab;
    public GameObject enemySpawnPrefab;
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPrefab;

    public GameObject bossAPrefab;
    public static float StageTime;

    private int beatsBeforeWave = 40;
    private int beats = 36;

    private int waves = 0;

    public List<Wave> partAWaves;
    public List<Wave> partBWaves;
    private int CurrentDifficultyPoints = 0;
    public int part; // 0 = partA, 1 = BossA, 2 = partB, 3 = BossB

    public static int EnemiesAlive, WaveNumberOfEnemies;

    public string stageID;

    public static bool isBossWave()
    {
        return Instance.part == 1 || Instance.part == 3;
    }

    private void Awake()
    {
        Instance = this;
        SetPools();
        BeatManager.SetTrack(tracks[0]);
        Player.ResetPosition();
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-1");
        EnemiesAlive = 0;
        WaveNumberOfEnemies = 0;
        waves = 0;
        StageTime = 0;
        part = 0;
        beats = 36;
    }
    void Start()
    {
        //StartCoroutine(SpawnEnemy());
    }

    public void SpawnNextWave(List<Wave> waveList)
    {
        CurrentDifficultyPoints = waves * GameManager.runData.currentLoop;
        List<Wave> possibleWaves = new List<Wave>();
        int minimumCost = CurrentDifficultyPoints - 2;
        int maximumCost = CurrentDifficultyPoints + 2;

        foreach (Wave wave in waveList) 
        {
            if (wave.getCost() == CurrentDifficultyPoints) possibleWaves.Add(wave);
        }

        if (possibleWaves.Count == 0)
        {
            foreach (Wave wave in waveList)
            {
                int cost = wave.getCost();
                if (cost > minimumCost && cost < maximumCost) possibleWaves.Add(wave);
            }
        }

        if (possibleWaves.Count == 0)
        {
            foreach (Wave wave in waveList)
            {
                possibleWaves.Add(wave);
            }
        }

        possibleWaves = ShuffleWaves(possibleWaves);

        possibleWaves[0].Spawn();
        waves++;
        AudioController.PlaySound(AudioController.instance.sounds.warningWaveSound);
        WaveNumberOfEnemies = possibleWaves[0].GetEnemyCount();
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

    public virtual void SetPools()
    {
        PoolManager.CreatePool(typeof(Gem), gemPrefab, 10);
        PoolManager.CreatePool(typeof(BulletGem), bulletgemPrefab, 100);
        PoolManager.CreatePool(typeof(TestEnemy), enemyPrefab, 10);
        PoolManager.CreatePool(typeof(TestBoss), bossAPrefab, 1);
        PoolManager.CreatePool(typeof(KillEffect), killEffectPrefab, 10);
        PoolManager.CreatePool(typeof(SpawnEffect), enemySpawnPrefab, 10);

        PoolManager.CreatePool(typeof(Bullet), bulletPrefab, 100);
        PoolManager.CreatePool(typeof(BulletSpawnEffect), bulletSpawnPrefab, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (BeatManager.isGameBeat && BeatManager.isPlaying)
        {
            beats++;
            if (part == 0)
            {
                if (waves < 21 && (beats >= beatsBeforeWave || EnemiesAlive < WaveNumberOfEnemies * 0.2f))
                {
                    beats = 0;
                    SpawnNextWave(partAWaves); // Normal A Wave
                }
                if (waves == 21 && EnemiesAlive == 0)
                {
                    beatsBeforeWave = 99999999;
                    StartCoroutine(StartBossASequence());
                    part = 1;
                }
            }

            if (part == 2)
            {
                if (waves < 42 && (beats >= beatsBeforeWave || EnemiesAlive < WaveNumberOfEnemies * 0.2f))
                {
                    beats = 0;
                    SpawnNextWave(partBWaves); // Normal A Wave
                }
                if (waves == 42 && EnemiesAlive == 0)
                {
                    beatsBeforeWave = 99999999;
                    StartCoroutine(StartBossBSequence());
                    part = 3;
                }
            }
        }
        StageTime += Time.deltaTime;
    }

    public static void StopMap()
    {
        Instance.RemoveAllPools();
        Instance.StopAllCoroutines();
    }

    public virtual void RemoveAllPools()
    {
        PoolManager.RemovePool(typeof(Gem));
        PoolManager.RemovePool(typeof(BulletGem));
        PoolManager.RemovePool(typeof(TestEnemy));
        PoolManager.RemovePool(typeof(TestBoss));
        PoolManager.RemovePool(typeof(KillEffect));
        PoolManager.RemovePool(typeof(SpawnEffect));

        PoolManager.RemovePool(typeof(Bullet));
        PoolManager.RemovePool(typeof(BulletSpawnEffect));
    }

    public static IEnumerator SpawnEnemyAroundPlayer(SpawnData spawnData, int index)
    {
        EnemiesAlive++;
        SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();
        float angle, x, y;
        while (true)
        {
            angle = Random.Range(0, 360f);
            x = Player.position.x + (Instance.Radius * Mathf.Cos(angle));
            y = Player.position.y + (Instance.Radius * Mathf.Sin(angle));
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
        EnemiesAlive++;
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
            x = Player.position.x + (Radius * Mathf.Cos(angle));
            y = Player.position.y + (Radius * Mathf.Sin(angle));
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
        UIManager.Instance.PlayerUI.UpdateBossBar(1000, 1000);
        UIManager.Instance.PlayerUI.SetBossBarName("TestBoss Name");
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stageboss")}");
        BeatManager.FadeOut();
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
        UIManager.Instance.PlayerUI.UpdateBossBar(1000, 1000);
        UIManager.Instance.PlayerUI.SetBossBarName("TestBoss Name");
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stageboss")}-2");
        BeatManager.FadeOut();
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

    public void OnBossDeath()
    {
        if (part == 1) StartCoroutine(BossADeathCoroutine());
        if (part == 3) StartCoroutine(VictoryCoroutine());
    }

    public virtual IEnumerator BossADeathCoroutine()
    {
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-2");
        BeatManager.FadeOut();
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
        BeatManager.FadeOut();
        yield return new WaitForSeconds(2f);

        // VICTORY MENU BEHAVIOUR HERE
        yield break;
    }
}
