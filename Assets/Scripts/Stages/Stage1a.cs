using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1a : Stage
{
    public GameObject fireflies;

    [Header("Enemy Prefabs")]
    public GameObject zombieThiefPrefab;
    public GameObject stayinUndeadPrefab;
    public GameObject stayingUndeadElitePrefab;
    public GameObject zombieBridePrefab;

    public GameObject onibiPrefab;
    public GameObject booJrPrefab;
    public GameObject booJrElitePrefab;

    public GameObject clawRiffPrefab;
    public GameObject skelekoPrefab;
    public GameObject purrfessorPrefab;
    public GameObject purrfessorElitePrefab;

    public GameObject rhythMaidenPrefab;
    public GameObject zippyBatPrefab;
    public GameObject vampiLoliElitePrefab;
    public GameObject ojouGuardianPrefab;
    public GameObject vampiloliTransform;

    public GameObject usarinBossPrefab;

    [Header("Other Prefabs")]
    public GameObject lightAttackPrefab;

    [SerializeField] List<Animator> stageLights;

    public override void Start()
    {
        fireflies.transform.parent = Camera.main.transform;
        fireflies.transform.localPosition = new Vector3(0, 0, 30);
    }

    public override void SetPools()
    {
        base.SetPools();
        PoolManager.CreatePool(typeof(ZombieThief), zombieThiefPrefab, 30);
        PoolManager.CreatePool(typeof(StayinUndead), stayinUndeadPrefab, 30);
        PoolManager.CreatePool(typeof(ZombieBride), zombieBridePrefab, 30);
        PoolManager.CreatePool(typeof(StayinUndeadElite), stayingUndeadElitePrefab, 1);

        PoolManager.CreatePool(typeof(Onibi), onibiPrefab, 30);
        PoolManager.CreatePool(typeof(JackO), booJrPrefab, 30);
        PoolManager.CreatePool(typeof(JackOElite), booJrElitePrefab, 1);

        PoolManager.CreatePool(typeof(ClawRiff), clawRiffPrefab, 30);
        PoolManager.CreatePool(typeof(Skeleko), skelekoPrefab, 30);
        PoolManager.CreatePool(typeof(Nekomander), purrfessorPrefab, 30);
        PoolManager.CreatePool(typeof(NekomanderElite), purrfessorElitePrefab, 1);

        PoolManager.CreatePool(typeof(RhythMaiden), rhythMaidenPrefab, 30);
        PoolManager.CreatePool(typeof(ZippyBat), zippyBatPrefab, 30);
        PoolManager.CreatePool(typeof(VampiLoliElite), vampiLoliElitePrefab, 1);
        PoolManager.CreatePool(typeof(OjouGuardian), ojouGuardianPrefab, 30);

        PoolManager.CreatePool(typeof(UsarinBoss), usarinBossPrefab, 1);
        PoolManager.CreatePool(typeof(LightAttack), lightAttackPrefab, 30);
        PoolManager.CreatePool(typeof(SmokeExplosion), vampiloliTransform, 1);
    }

    public override void RemoveAllPools()
    {
        Debug.Log("removed pools");
        base.RemoveAllPools();
        PoolManager.RemovePool(typeof(ZombieThief)); // 0s - 4 min
        PoolManager.RemovePool(typeof(StayinUndead)); // 30s - 2 min
        PoolManager.RemovePool(typeof(ZombieBride)); // 1 min - 2:30 min
        PoolManager.RemovePool(typeof(StayinUndeadElite)); // Minute 2

        PoolManager.RemovePool(typeof(Onibi));
        PoolManager.RemovePool(typeof(JackO)); // Min 2 - min 4:30
        PoolManager.RemovePool(typeof(JackOElite)); // Minute 4

        PoolManager.RemovePool(typeof(ClawRiff));
        PoolManager.RemovePool(typeof(Skeleko)); // Minute 4:30 - 6:30
        PoolManager.RemovePool(typeof(Nekomander)); // Minute 5:00 - 6:00
        PoolManager.RemovePool(typeof(NekomanderElite)); // Minute 6

        PoolManager.RemovePool(typeof(RhythMaiden)); // 6:30 - 8
        PoolManager.RemovePool(typeof(ZippyBat)); // 7:00 - 9:30
        PoolManager.RemovePool(typeof(VampiLoliElite)); // Minute 8
        PoolManager.RemovePool(typeof(OjouGuardian)); // Spawn in waves while the miniboss without VampiLoli, use the Usarin method
        
        PoolManager.RemovePool(typeof(UsarinBoss));
        PoolManager.RemovePool(typeof(LightAttack));
        PoolManager.RemovePool(typeof(SmokeExplosion));
    }

    public override void OnStopMap()
    {
        fireflies.transform.parent = null;
        SceneManager.MoveGameObjectToScene(fireflies, gameObject.scene);
    }
    public override void OnBossFightStart(Boss boss)
    {
        foreach (Animator anim in stageLights)
        {
            anim.Play("stagelight_usarin");
        }
    }
    protected override IEnumerator BossDefeatCoroutine(Boss boss)
    {
        foreach (Animator anim in stageLights)
        {
            anim.Play("stagelight_off");
        }
        Player.instance.StopAllCoroutines();
        // This is supposed to be an animation
        ForceDespawnBullets();
        Player.instance.canDoAnything = false;
        PlayerCamera.instance.followPlayer = false;
        Player.instance.ForceDespawnAbilities(false);
        Player.instance.ResetAbilities();
        Player.instance.StopAllCoroutines();
        Player.instance.rb.velocity = Vector2.zero;
        if (Player.instance is PlayerRabi) Player.instance.animator.Play("Rabi_Idle");
        yield return new WaitForSeconds(0.45f); // This is when the white screen is pure white
        BeatManager.FadeOut(1);
        currentBoss = null;
        
        Player.instance.facingRight = true;
        Player.instance.Sprite.transform.localScale = Vector3.one;
        
        //bossGrid.SetActive(false);
        bossArea.color = new Color(0, 0, 0, 0);
        bossArea.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        bossArea.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        UIManager.Instance.cutsceneManager.StartCutscene(CutsceneType.StageEnd);
        yield return null;
        PlayerCamera.instance.SetCameraPos(bossArea.transform.position);
        PoolManager.Return(boss.gameObject, boss.GetType());
        while (!UIManager.Instance.cutsceneManager.hasFinished) yield return null;
        yield return new WaitForSeconds(1f);
        UIManager.Instance.cutsceneManager.hasFinished = false;

        UIManager.Instance.StageFinish.SetActive(true);
        AudioController.PlayMusic(AudioController.instance.sounds.stageComplete, false);

        yield return new WaitForSeconds(6);

        OnStopMap();
        UIManager.Fade(false);
        UIManager.Instance.StageFinish.SetActive(false);
        yield return new WaitForSeconds(1f);
        GameManager.runData.stageMulti++;
        Player.instance.isMoving = false;
        GameManager.LoadNextStage("Stage1b");
        yield break;
    }
    protected override void StartMapWaveList()
    {
        possibleEventTypes = new List<StageEventType>()
        {
            StageEventType.SpawnSpreadGroup,
            StageEventType.SpawnCircleHordeGroup,
            StageEventType.SpawnElite,
            StageEventType.SpawnBoss,
            StageEventType.Wait
        };

        patternNumber = 1;
        /*
        foreach (StageWave wave in Instance.waves)
        {
            wave.Initialize();
        }
        */
        /*
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-1");
        stageEvents = new List<StageEvent>()
        {
            // Spawn Rates
            
            new ChangeSpawnRateEvent(2, 0), // start
            new ChangeSpawnRateEvent(2, 30),
            new ChangeSpawnRateEvent(3, 60),
            new ChangeSpawnRateEvent(4, 90),
            new ChangeSpawnRateEvent(5, 120), // first elite
            new ChangeSpawnRateEvent(2, 150),
            new ChangeSpawnRateEvent(3, 180),
            new ChangeSpawnRateEvent(3, 210),
            new ChangeSpawnRateEvent(4, 240), // second elite
            new ChangeSpawnRateEvent(2, 270),
            new ChangeSpawnRateEvent(3, 300),
            new ChangeSpawnRateEvent(4, 330),
            new ChangeSpawnRateEvent(5, 360), // third elite
            new ChangeSpawnRateEvent(2, 390),
            new ChangeSpawnRateEvent(3, 420),
            new ChangeSpawnRateEvent(4, 450),
            new ChangeSpawnRateEvent(5, 480), // 4 elite
            new ChangeSpawnRateEvent(4, 510),
            new ChangeSpawnRateEvent(5, 540),
            new ChangeSpawnRateEvent(6, 570),
            new ChangeSpawnRateEvent(0, 595), // boss

            new ChangeSpawnCooldownEvent(8, 0), // 12
            new ChangeSpawnCooldownEvent(7, 30), // 10
            new ChangeSpawnCooldownEvent(7, 120), // 12
            new ChangeSpawnCooldownEvent(6, 150), // 11
            new ChangeSpawnCooldownEvent(5, 180), // 11
            new ChangeSpawnCooldownEvent(5, 240), // 11
            new ChangeSpawnCooldownEvent(4, 300), // 11
            new ChangeSpawnCooldownEvent(5, 360), // 9
            new ChangeSpawnCooldownEvent(4, 480), // 12
            new ChangeSpawnCooldownEvent(4, 540), // 9
            /*
            new AddEnemyEvent(EnemyType.ZombieThief, 6, 0, 0), // nomslime
            new AddEnemyEvent(EnemyType.StayinUndead, 4, 0, 30),
            new AddEnemyEvent(EnemyType.ZombieBride, 2, 0, 60),

            new AddEnemyEvent(EnemyType.BooJr, 4, 0, 120),
            new AddEnemyEvent(EnemyType.Onibi, 1, 0, 150),
 
            new AddEnemyEvent(EnemyType.Skeleko, 6, 0, 270),
            new AddEnemyEvent(EnemyType.ClawRiff, 4, 0, 300),
            new AddEnemyEvent(EnemyType.Purrfessor, 1, 0, 330),

            new AddEnemyEvent(EnemyType.ZippyBat, 6, 0, 390),
            new AddEnemyEvent(EnemyType.RhythMaiden, 1, 0, 420),

            new RemoveEnemyEvent(EnemyType.ZombieThief, 0, 220),
            new RemoveEnemyEvent(EnemyType.StayinUndead, 0, 120),
            new RemoveEnemyEvent(EnemyType.ZombieBride, 0, 150),

            new RemoveEnemyEvent(EnemyType.BooJr, 0, 270),
            new RemoveEnemyEvent(EnemyType.Onibi, 0, 330),

            new RemoveEnemyEvent(EnemyType.ClawRiff, 0, 420),
            new RemoveEnemyEvent(EnemyType.Skeleko, 0, 390),
            new RemoveEnemyEvent(EnemyType.Purrfessor, 0, 360),

            new RemoveEnemyEvent(EnemyType.ZippyBat, 0, 598),
            new RemoveEnemyEvent(EnemyType.RhythMaiden, 0, 598),

            new SpawnEliteEvent(EnemyType.StayinUndeadElite, 120),
            new SpawnEliteEvent(EnemyType.BooJrElite, 240), // 240
            new SpawnEliteEvent(EnemyType.PurrfessorElite, 360), // 360
            new SpawnEliteEvent(EnemyType.VampiLoliElite, 480), // 480
            
            new SpawnBossEvent(EnemyType.Usarin, 600), // 600
            new SpawnCircleHordeEvent(new SpawnData() { AItype = EnemyAIType.CircleHorde, enemyType = EnemyType.ZombieThief, spawnType = SpawnType.AROUND_PLAYER }, 15, 5)

        };*/


    }
}