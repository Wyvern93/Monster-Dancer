using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Stage1b : Map
{
    public GameObject fireflies;

    [Header("Enemy Prefabs")]
    public GameObject wiggleViperPrefab;
    public GameObject muscleHarePrefab;
    public GameObject muscleHareElitePrefab;
    public GameObject rhythmiaPrefab;
    public GameObject fungooPrefab;
    public GameObject rhythmiaElitePrefab;
    public GameObject fungooElitePrefab;
    public GameObject stayinundeadPrefab;
    public GameObject purrfessorPrefab;
    public GameObject stayinundeadElitePrefab;
    public GameObject zippyBatPrefab;
    public GameObject vampiLoliPrefab;
    public GameObject ojouGuaridanPrefab;
    public GameObject kappaPrefab;

    public GameObject nebulionBossPrefab;

    public override void Start()
    {
        fireflies.transform.parent = Camera.main.transform;
    }

    public override void SetPools()
    {
        base.SetPools();
        PoolManager.CreatePool(typeof(WiggleViper), wiggleViperPrefab, 50);
        PoolManager.CreatePool(typeof(MuscleHare), muscleHarePrefab, 50);
        PoolManager.CreatePool(typeof(MuscleHareElite), muscleHareElitePrefab, 1);
        PoolManager.CreatePool(typeof(Rhythmia), rhythmiaPrefab, 50);
        PoolManager.CreatePool(typeof(Fungoo), fungooPrefab, 50);
        PoolManager.CreatePool(typeof(RhythmiaElite), rhythmiaElitePrefab, 1);
        PoolManager.CreatePool(typeof(FungooElite), fungooElitePrefab, 1);
        PoolManager.CreatePool(typeof(StayinUndead), stayinundeadPrefab, 50);
        PoolManager.CreatePool(typeof(StayinUndeadElite), stayinundeadElitePrefab, 1);
        PoolManager.CreatePool(typeof(Purrfessor), purrfessorPrefab, 50);
        PoolManager.CreatePool(typeof(ZippyBat), zippyBatPrefab, 50);
        PoolManager.CreatePool(typeof(VampiLoli), vampiLoliPrefab, 30);
        PoolManager.CreatePool(typeof(OjouGuardian), ojouGuaridanPrefab, 50);
        PoolManager.CreatePool(typeof(Kappa), kappaPrefab, 30);
        PoolManager.CreatePool(typeof(NebulionBoss), nebulionBossPrefab, 1);
    }

    public override void RemoveAllPools()
    {
        Debug.Log("removed pools");
        base.RemoveAllPools();
        PoolManager.RemovePool(typeof(WiggleViper));
        PoolManager.RemovePool(typeof(MuscleHare));
        PoolManager.RemovePool(typeof(MuscleHareElite));
        PoolManager.RemovePool(typeof(Rhythmia));
        PoolManager.RemovePool(typeof(Fungoo));
        PoolManager.RemovePool(typeof(RhythmiaElite));
        PoolManager.RemovePool(typeof(FungooElite));
        PoolManager.RemovePool(typeof(StayinUndead));
        PoolManager.RemovePool(typeof(StayinUndeadElite));
        PoolManager.RemovePool(typeof(ZippyBat));
        PoolManager.RemovePool(typeof(Purrfessor));
        PoolManager.RemovePool(typeof(VampiLoli));
        PoolManager.RemovePool(typeof(OjouGuardian));
        PoolManager.RemovePool(typeof(Kappa));
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
        GameManager.LoadNextStage("Stage1a");
        yield break;
    }
    protected override void StartMapEventListA()
    {
        UIManager.Instance.PlayerUI.SetStageText($"{Localization.GetLocalizedString("playerui.stage")} {stageID}-2");
        stageEvents = new List<StageTimeEvent>()
        {
            // Spawn Rates
            
            new ChangeSpawnRateEvent(7, 0), // start
            new ChangeSpawnRateEvent(7, 30),
            new ChangeSpawnRateEvent(8, 60),
            new ChangeSpawnRateEvent(9, 90),
            new ChangeSpawnRateEvent(5, 120), // first elite
            new ChangeSpawnRateEvent(7, 150),
            new ChangeSpawnRateEvent(8, 180),
            new ChangeSpawnRateEvent(9, 210),
            new ChangeSpawnRateEvent(7, 240), // second elite
            new ChangeSpawnRateEvent(9, 270),
            new ChangeSpawnRateEvent(10, 300),
            new ChangeSpawnRateEvent(10, 330),
            new ChangeSpawnRateEvent(8, 360), // third elite
            new ChangeSpawnRateEvent(8, 390),
            new ChangeSpawnRateEvent(8, 420),
            new ChangeSpawnRateEvent(9, 450),
            new ChangeSpawnRateEvent(7, 480), // 4 elite
            new ChangeSpawnRateEvent(8, 510),
            new ChangeSpawnRateEvent(9, 540),
            new ChangeSpawnRateEvent(10, 570),
            new ChangeSpawnRateEvent(0, 595), // boss

            new ChangeSpawnCooldownEvent(10, 0),
            new ChangeSpawnCooldownEvent(9, 30),
            new ChangeSpawnCooldownEvent(11, 120),
            new ChangeSpawnCooldownEvent(10, 180),
            new ChangeSpawnCooldownEvent(12, 240),
            new ChangeSpawnCooldownEvent(9, 360),
            new ChangeSpawnCooldownEvent(8, 480),
            new ChangeSpawnCooldownEvent(8, 540),
            
            new AddEnemyEvent(EnemyType.WiggleViper, 5, 0, 0), // wiggleviper
            new AddEnemyEvent(EnemyType.MuscleHare, 1, 0, 30),
            new AddEnemyEvent(EnemyType.Rhytmia, 1, 0, 150),
            new AddEnemyEvent(EnemyType.Fungoo, 4, 0, 200),
            new AddEnemyEvent(EnemyType.StayinUndead, 3, 0, 300),
            new AddEnemyEvent(EnemyType.Purrfessor, 1, 0, 330),
            new AddEnemyEvent(EnemyType.ZippyBat, 8, 0, 400), // Needs custom spawn
            new AddEnemyEvent(EnemyType.Kappa, 1, 0, 530),

            new RemoveEnemyEvent(EnemyType.WiggleViper, 0, 180),
            new RemoveEnemyEvent(EnemyType.MuscleHare, 0, 300),
            new RemoveEnemyEvent(EnemyType.Rhytmia, 0, 300),
            new RemoveEnemyEvent(EnemyType.Fungoo, 0, 360),
            new RemoveEnemyEvent(EnemyType.StayinUndead, 0, 420),
            new RemoveEnemyEvent(EnemyType.Purrfessor, 0, 540),
            new RemoveEnemyEvent(EnemyType.ZippyBat, 0, 600),
            new RemoveEnemyEvent(EnemyType.VampiLoli, 0, 540),
            new RemoveEnemyEvent(EnemyType.OjouGuardian, 0, 570),
            new RemoveEnemyEvent(EnemyType.Kappa, 0, 600),
            
            new SpawnEliteEvent(EnemyType.MuscleHareElite, 120), // 120
            new SpawnEliteEvent(EnemyType.RhytmiaElite, 240), // 240
            new SpawnEliteEvent(EnemyType.FungooElite, 360), // 360
            new SpawnEliteEvent(EnemyType.StayinUndeadElite, 480), // 480
            
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 400),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 420),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 440),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 460),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 480),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 500),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 510),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 520),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 530),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 535),
            new SpawnUniqueEnemyEvent(EnemyType.VampiLoli, 540),
            
            new SpawnBossEvent(EnemyType.Nebulion, 600)
        };
    }
}