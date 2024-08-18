using System;
using UnityEngine;

[Serializable]
public class SpawnData
{
    public float weight;
    public EnemyType enemyType;
    public SpawnType spawnType;
    public Vector2 spawnPosition;
    public int AItype;
}