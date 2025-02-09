using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Stage1b : Stage
{
    public GameObject fireflies;

    [Header("Enemy Prefabs")]
    public GameObject nomSlimePrefab;
    public GameObject poisyPrefab;
    public GameObject slimeDancerPrefab;
    public GameObject nomSlimeElitePrefab;
    public GameObject kappaPrefab;
    public GameObject tanukiPrefab;
    public GameObject fungooPrefab;
    public GameObject fungooElitePrefab;
    public GameObject buzzBeePrefab;
    public GameObject troncoPrefab;
    public GameObject dancearunePrefab;
    public GameObject dancearuneElitePrefab;
    public GameObject wiggleViperPrefab;
    public GameObject rhythmiaPrefab;
    public GameObject karakasaPrefab;
    public GameObject rhythmiaElitePrefab;
    

    public GameObject nebulionBossPrefab;

    public override void Start()
    {
        fireflies.transform.parent = Camera.main.transform;
    }

    public override void SetPools()
    {
        base.SetPools();
        PoolManager.CreatePool(typeof(NomSlime), nomSlimePrefab, 50);
        PoolManager.CreatePool(typeof(Poisy), poisyPrefab, 50);
        PoolManager.CreatePool(typeof(SlimeDancer), slimeDancerPrefab, 50);
        PoolManager.CreatePool(typeof(NomSlimeElite), nomSlimeElitePrefab, 1);
        PoolManager.CreatePool(typeof(Kappa), kappaPrefab, 50);
        PoolManager.CreatePool(typeof(Tanuki), tanukiPrefab, 50);
        PoolManager.CreatePool(typeof(Fungoo), fungooPrefab, 50);
        PoolManager.CreatePool(typeof(FungooElite), fungooElitePrefab, 1);
        PoolManager.CreatePool(typeof(BuzzBee), buzzBeePrefab, 50);
        PoolManager.CreatePool(typeof(Tronco), troncoPrefab, 50);
        PoolManager.CreatePool(typeof(Dancearune), dancearunePrefab, 50);
        PoolManager.CreatePool(typeof(DancearuneElite), dancearuneElitePrefab, 1);
        PoolManager.CreatePool(typeof(WiggleViper), wiggleViperPrefab, 50);
        PoolManager.CreatePool(typeof(Rhythmia), rhythmiaPrefab, 50);
        PoolManager.CreatePool(typeof(Karakasa), karakasaPrefab, 50);
        PoolManager.CreatePool(typeof(RhythmiaElite), rhythmiaElitePrefab, 1);

        PoolManager.CreatePool(typeof(NebulionBoss), nebulionBossPrefab, 1);
    }

    public override void RemoveAllPools()
    {
        Debug.Log("removed pools");
        base.RemoveAllPools();
        PoolManager.RemovePool(typeof(NomSlime));
        PoolManager.RemovePool(typeof(Poisy));
        PoolManager.RemovePool(typeof(SlimeDancer));
        PoolManager.RemovePool(typeof(NomSlimeElite));
        PoolManager.RemovePool(typeof(Kappa));
        PoolManager.RemovePool(typeof(Tanuki));
        PoolManager.RemovePool(typeof(Fungoo));
        PoolManager.RemovePool(typeof(FungooElite));
        PoolManager.RemovePool(typeof(BuzzBee));
        PoolManager.RemovePool(typeof(Tronco));
        PoolManager.RemovePool(typeof(Dancearune));
        PoolManager.RemovePool(typeof(DancearuneElite));
        PoolManager.RemovePool(typeof(WiggleViper));
        PoolManager.RemovePool(typeof(Rhythmia));
        PoolManager.RemovePool(typeof(Karakasa));
        PoolManager.RemovePool(typeof(RhythmiaElite));
        PoolManager.RemovePool(typeof(NebulionBoss));
    }

    public override void OnStopMap()
    {
        fireflies.transform.parent = null;
        SceneManager.MoveGameObjectToScene(fireflies, gameObject.scene);
    }

    protected override IEnumerator BossDefeatCoroutine(Boss boss)
    {
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

        mapObjects.SetActive(false);
        bossGrid.SetActive(false);
        bossArea.color = new Color(0, 0, 0, 0);
        bossArea.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        bossArea.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        UIManager.Instance.cutsceneManager.StartCutscene(CutsceneType.StageEnd);
        yield return null;
        PlayerCamera.instance.SetCameraPos(CutsceneAnimator.transform.position);
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
        GameManager.LoadNextStage("Stage1a");
        yield break;
    }
    protected override void StartMapWaveList()
    {/*
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-2");
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
            
            new AddEnemyEvent(EnemyType.NomSlime, 6, 0, 0), // nomslime
            new AddEnemyEvent(EnemyType.Poisy, 3, 0, 30),
            new AddEnemyEvent(EnemyType.SlimeDancer, 1, 0, 60),

            new AddEnemyEvent(EnemyType.Kappa, 1, 0, 130),
            new AddEnemyEvent(EnemyType.Tanuki, 5, 0, 160),
            new AddEnemyEvent(EnemyType.Fungoo, 3, 0, 200),

            new AddEnemyEvent(EnemyType.BuzzBee, 4, 0, 310),
            new AddEnemyEvent(EnemyType.Tronco, 6, 0, 250),
            new AddEnemyEvent(EnemyType.Dancearune, 1, 0, 290),

            new AddEnemyEvent(EnemyType.WiggleViper, 5, 0, 380),
            new AddEnemyEvent(EnemyType.Rhytmia, 1, 0, 410),
            new AddEnemyEvent(EnemyType.Karakasa, 3, 0, 440),

            new RemoveEnemyEvent(EnemyType.NomSlime, 0, 120),
            new RemoveEnemyEvent(EnemyType.Poisy, 0, 130),
            new RemoveEnemyEvent(EnemyType.SlimeDancer, 0, 140),

            new RemoveEnemyEvent(EnemyType.Kappa, 0, 220),
            new RemoveEnemyEvent(EnemyType.Tanuki, 0, 260),
            new RemoveEnemyEvent(EnemyType.Fungoo, 0, 280),

            new RemoveEnemyEvent(EnemyType.BuzzBee, 0, 380),
            new RemoveEnemyEvent(EnemyType.Tronco, 0, 390),
            new RemoveEnemyEvent(EnemyType.Dancearune, 0, 375),

            new RemoveEnemyEvent(EnemyType.WiggleViper, 0, 560),
            new RemoveEnemyEvent(EnemyType.Rhytmia, 0, 598),
            new RemoveEnemyEvent(EnemyType.Karakasa, 0, 598),

            new SpawnEliteEvent(EnemyType.NomSlimeElite, 120),
            new SpawnEliteEvent(EnemyType.FungooElite, 240), // 240
            new SpawnEliteEvent(EnemyType.DancearuneElite, 360), // 360
            new SpawnEliteEvent(EnemyType.RhytmiaElite, 480), // 480
            
            new SpawnBossEvent(EnemyType.Nebulion, 600)
        };
    */}
}