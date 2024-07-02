using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Map : MonoBehaviour
{
    public static Map Instance;
    public LayerMask nonPassableMask;
    public GameObject enemyPrefab;
    float Radius = 6f;

    public AudioClip enemyDeathSound;
    public AudioClip enemyHurtSound;
    public AudioClip gemSound;
    public AudioClip warningWaveSound;
    public AudioClip grazeSound;

    public GameObject gemPrefab;
    public GameObject bulletgemPrefab;
    public GameObject killEffectPrefab;
    public GameObject enemySpawnPrefab;
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPrefab;
    public static float StageTime;

    private int beatsBeforeWave = 40;
    private int beats = 36;

    private int enemiesPerWave = 4;
    private int waves = 0;

    private void Awake()
    {
        Instance = this;
        SetPools();
    }
    void Start()
    {
        //StartCoroutine(SpawnEnemy());
    }

    private void SetPools()
    {
        PoolManager.CreatePool(typeof(Gem), gemPrefab, 10);
        PoolManager.CreatePool(typeof(BulletGem), bulletgemPrefab, 100);
        PoolManager.CreatePool(typeof(TestEnemy), enemyPrefab, 10);
        PoolManager.CreatePool(typeof(KillEffect), killEffectPrefab, 10);
        PoolManager.CreatePool(typeof(SpawnEffect), enemySpawnPrefab, 10);

        PoolManager.CreatePool(typeof(Bullet), bulletPrefab, 100);
        PoolManager.CreatePool(typeof(BulletSpawnEffect), bulletSpawnPrefab, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (BeatManager.isGameBeat)
        {
            beats++;
            if (beats == beatsBeforeWave)
            {
                beats = 0;
                beatsBeforeWave--;
                for (int i = 0; i < enemiesPerWave; i++)
                {
                    AudioController.PlaySound(warningWaveSound);
                    StartCoroutine(SpawnEnemy());
                }
                if (waves % 3 == 0)
                {
                    enemiesPerWave = Mathf.Clamp(enemiesPerWave + 1, 1, 16);
                }
            }
            
        }
        StageTime += Time.deltaTime;
    }

    private IEnumerator SpawnEnemy()
    {
        SpawnEffect spawnEffect = PoolManager.Get<SpawnEffect>();
        float angle, x, y;
        while (true)
        {
            angle = Random.Range(0, 360f);
            x = Player.position.x + (Radius * Mathf.Cos(angle));
            y = Player.position.y + (Radius * Mathf.Sin(angle));
            x = Mathf.Clamp(x, -19f, 18f);
            y = Mathf.Clamp(y, -11f, 10f);
            if (!isWallAt(new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y)))) break;
        }
        spawnEffect.transform.position = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        yield return new WaitForSeconds(BeatManager.GetBeatDuration() * 2);

        PoolManager.Return(spawnEffect.gameObject, typeof(SpawnEffect));

        Enemy enemy = PoolManager.Get<TestEnemy>();
        enemy.transform.position = new Vector3(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
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
}
