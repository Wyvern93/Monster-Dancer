using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestingEnvironment : MonoBehaviour
{
    public TestingData TestingData;
    public static TestingEnvironment Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.LoadScene("Core", LoadSceneMode.Additive);
        StartCoroutine(WaitAndStart());
    }

    private IEnumerator WaitAndStart()
    {
        yield return new WaitUntil(() => GameManager.instance != null);
        yield return new WaitUntil(() => GameManager.isLoading == true);

        LoadPools(TestingData.poolLists);

        // Add Player
        GameManager.runData.ultimateChosen = TestingData.ultimateSkill;
        GameManager.LoadPlayer(TestingData.playerPrefab);
        BeatManager.SetTrack(TestingData.track);
        Player.ResetPosition();
        PlayerCamera.instance.SetCameraPos(Vector3.zero);

        // Set UI to Gameplay
        UIManager.Fade(true);
        UIManager.Instance.PlayerUI.CreatePools();
        BeatManager.StartTrack();
        UIManager.Instance.PlayerUI.ShowUI();
        UIManager.Instance.PlayerUI.OnCloseMenu();
        Player.instance.canDoAnything = true;
        GameManager.isLoading = false;
        GameManager.isPaused = false;

        yield return null;
        Player.instance.transform.position = new Vector3(0, -3, 0);
        yield return null;
        SpawnEnemy(TestingData.enemyToSpawn, TestingData.enemyAIType);
        yield return null;
    }

    private void LoadPools(List<PoolList> poolLists)
    {
        foreach (PoolList list in poolLists)
        {
            foreach (PoolData pool in list.pools)
            {
                pool.CreatePool();
            }
        }
    }

    private void SpawnEnemy(EnemyType enemyType, EnemyAIType aiType)
    {
        if (Enemy.enemyData[enemyType].enemyClass == EnemyClass.Elite)
        {
            Stage.SpawnElite(new SpawnData() { enemyType = enemyType, spawnPosition = Vector3.zero });
        }
        else if (Enemy.enemyData[enemyType].enemyClass == EnemyClass.Boss)
        {
            Stage.Instance.SpawnBoss(new SpawnData() { enemyType = enemyType, spawnPosition = Vector3.zero });
        }
    }
}