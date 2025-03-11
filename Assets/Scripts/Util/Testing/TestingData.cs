using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Testing/TestingData", fileName = "TestingData")][Serializable]
public class TestingData : ScriptableObject
{
    [Header("Player Data")]
    public GameObject playerPrefab;
    public List<PlayerAbilityID> startingSkills;
    public PlayerAbilityID ultimateSkill; 

    [Header("Music Data")]
    public MapTrack track;

    [Header("Stage Data")]
    public List<PoolList> poolLists;

    [Header("Enemy Spawn")]
    public EnemyType enemyToSpawn;
    public EnemyAIType enemyAIType;
}