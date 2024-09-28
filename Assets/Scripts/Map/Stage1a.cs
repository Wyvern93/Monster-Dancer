using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1a : Map
{
    public GameObject fireflies;

    [Header("Enemy Prefabs")]
    public GameObject nomslimePrefab;
    public GameObject slimedancerPrefab;
    public GameObject nomslimeElitePrefab;
    public GameObject booJrPrefab;
    public GameObject zombieThiefPrefab;
    public GameObject booJrElitePrefab;
    public GameObject poisyPrefab;
    public GameObject troncoPrefab;
    public GameObject dancearunePrefab;
    public GameObject poisyElitePrefab;
    public GameObject skelekoPrefab;
    public GameObject zombiebridePrefab;
    public GameObject dancearuneElitePrefab;
    public GameObject carrotfanPrefab;
    public GameObject usarinRunningPrefab;

    public GameObject usarinBossPrefab;

    public override void Start()
    {
        fireflies.transform.parent = Camera.main.transform;
    }

    public override void SetPools()
    {
        base.SetPools();
        PoolManager.CreatePool(typeof(NomSlime), nomslimePrefab, 30);
        PoolManager.CreatePool(typeof(SlimeDancer), slimedancerPrefab, 30);
        PoolManager.CreatePool(typeof(NomSlimeElite), nomslimeElitePrefab, 1);
        PoolManager.CreatePool(typeof(GhostJr), booJrPrefab, 30);
        PoolManager.CreatePool(typeof(ZombieThief), zombieThiefPrefab, 30);
        PoolManager.CreatePool(typeof(GhostJrElite), booJrElitePrefab, 1);
        PoolManager.CreatePool(typeof(Poisy), poisyPrefab, 30);
        PoolManager.CreatePool(typeof(Tronco), troncoPrefab, 30);
        PoolManager.CreatePool(typeof(Dancearune), dancearunePrefab, 30);
        PoolManager.CreatePool(typeof(PoisyElite), poisyElitePrefab, 1);
        PoolManager.CreatePool(typeof(Skeleko), skelekoPrefab, 30);
        PoolManager.CreatePool(typeof(ZombieBride), zombiebridePrefab, 30);
        PoolManager.CreatePool(typeof(DancearuneElite), dancearuneElitePrefab, 1);
        PoolManager.CreatePool(typeof(CarrotFan), carrotfanPrefab, 30);
        PoolManager.CreatePool(typeof(UsarinRunning), usarinRunningPrefab, 5);
        PoolManager.CreatePool(typeof(UsarinBoss), usarinBossPrefab, 1);
    }

    public override void RemoveAllPools()
    {
        Debug.Log("removed pools");
        base.RemoveAllPools();
        PoolManager.RemovePool(typeof(NomSlime));
        PoolManager.RemovePool(typeof(SlimeDancer));
        PoolManager.RemovePool(typeof(NomSlimeElite));
        PoolManager.RemovePool(typeof(GhostJr));
        PoolManager.RemovePool(typeof(ZombieThief));
        PoolManager.RemovePool(typeof(GhostJrElite));
        PoolManager.RemovePool(typeof(Poisy));
        PoolManager.RemovePool(typeof(Tronco));
        PoolManager.RemovePool(typeof(Dancearune));
        PoolManager.RemovePool(typeof(PoisyElite));
        PoolManager.RemovePool(typeof(Skeleko));
        PoolManager.RemovePool(typeof(ZombieBride));
        PoolManager.RemovePool(typeof(DancearuneElite));
        PoolManager.RemovePool(typeof(CarrotFan));
        PoolManager.RemovePool(typeof(UsarinRunning));
        PoolManager.RemovePool(typeof(UsarinBoss));
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

        Dialogue dialogue = Player.instance is PlayerRabi ? rabiEndDialogue : rabiEndDialogue;
        UIManager.Instance.dialogueMenu.StartCutscene(dialogue.entries);
        yield return new WaitForEndOfFrame();
        Camera.main.transform.position = new Vector3(CutsceneAnimator.transform.position.x, CutsceneAnimator.transform.position.y, -60);
        PoolManager.Return(boss.gameObject, boss.GetType());
        while (!UIManager.Instance.dialogueMenu.hasFinished) yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.dialogueMenu.hasFinished = false;

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
    protected override void StartMapEventListA()
    {
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-1");
        stageEvents = new List<StageTimeEvent>()
        {
            // Spawn Rates
            
            new ChangeSpawnRateEvent(5, 0), // start
            new ChangeSpawnRateEvent(7, 30),
            new ChangeSpawnRateEvent(9, 60),
            new ChangeSpawnRateEvent(9, 90),
            new ChangeSpawnRateEvent(5, 120), // first elite
            new ChangeSpawnRateEvent(9, 150),
            new ChangeSpawnRateEvent(10, 180),
            new ChangeSpawnRateEvent(10, 210),
            new ChangeSpawnRateEvent(7, 240), // second elite
            new ChangeSpawnRateEvent(9, 270),
            new ChangeSpawnRateEvent(10, 300),
            new ChangeSpawnRateEvent(11, 330),
            new ChangeSpawnRateEvent(7, 360), // third elite
            new ChangeSpawnRateEvent(8, 390),
            new ChangeSpawnRateEvent(9, 420),
            new ChangeSpawnRateEvent(10, 450),
            new ChangeSpawnRateEvent(7, 480), // 4 elite
            new ChangeSpawnRateEvent(9, 510),
            new ChangeSpawnRateEvent(10, 540),
            new ChangeSpawnRateEvent(11, 570),
            new ChangeSpawnRateEvent(0, 595), // boss

            new ChangeSpawnCooldownEvent(14, 0),
            new ChangeSpawnCooldownEvent(10, 30),
            new ChangeSpawnCooldownEvent(12, 120),
            new ChangeSpawnCooldownEvent(10, 180),
            new ChangeSpawnCooldownEvent(8, 240),
            new ChangeSpawnCooldownEvent(6, 360),
            new ChangeSpawnCooldownEvent(12, 480),
            new ChangeSpawnCooldownEvent(8, 540),

            new AddEnemyEvent(EnemyType.NomSlime, 5, 0, 0), // nomslime
            new AddEnemyEvent(EnemyType.SlimeDancer, 1, 0, 30),
            new AddEnemyEvent(EnemyType.ZombieThief, 5, 0, 150),
            new AddEnemyEvent(EnemyType.BooJr, 1, 0, 200),
            new AddEnemyEvent(EnemyType.Poisy, 3, 0, 300),
            new AddEnemyEvent(EnemyType.Tronco, 4, 0, 330),
            new AddEnemyEvent(EnemyType.Dancearune, 1, 0, 360),
            new AddEnemyEvent(EnemyType.Skeleko, 3, 0, 390),
            new AddEnemyEvent(EnemyType.ZombieBride, 1, 0, 450),

            new RemoveEnemyEvent(EnemyType.NomSlime, 0, 180),
            new RemoveEnemyEvent(EnemyType.SlimeDancer, 0, 300),
            new RemoveEnemyEvent(EnemyType.ZombieThief, 0, 300),
            new RemoveEnemyEvent(EnemyType.BooJr, 0, 360),
            new RemoveEnemyEvent(EnemyType.Poisy, 0, 480),
            new RemoveEnemyEvent(EnemyType.Tronco, 0, 510),
            new RemoveEnemyEvent(EnemyType.Dancearune, 0, 520),
            new RemoveEnemyEvent(EnemyType.Skeleko, 0, 540),
            new RemoveEnemyEvent(EnemyType.ZombieBride, 0, 540),
            
            new SpawnEliteEvent(EnemyType.NomSlimeElite, 120),
            
            new SpawnEliteEvent(EnemyType.BooJrElite, 240), // 240
            new SpawnEliteEvent(EnemyType.PoisyElite, 360), // 360
            new SpawnEliteEvent(EnemyType.DancearuneElite, 480), // 480
            
            new SpawnUniqueEnemyEvent(EnemyType.UsarinRunning, 520),
            new SpawnUniqueEnemyEvent(EnemyType.UsarinRunning, 540),
            new SpawnUniqueEnemyEvent(EnemyType.UsarinRunning, 560),
            new SpawnUniqueEnemyEvent(EnemyType.UsarinRunning, 570),
            new SpawnUniqueEnemyEvent(EnemyType.UsarinRunning, 580),
            new SpawnUniqueEnemyEvent(EnemyType.UsarinRunning, 585),
            new SpawnUniqueEnemyEvent(EnemyType.UsarinRunning, 590),
            new SpawnUniqueEnemyEvent(EnemyType.UsarinRunning, 595),
            
            new SpawnBossEvent(EnemyType.Usarin, 600), // 600

        };
    }
}