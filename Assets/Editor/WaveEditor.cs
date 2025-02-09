using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{/*
    float StartingLevel, EndLevel;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Wave wave = (Wave)target;

        int exp = 0;
        foreach (SpawnCircleHordeEvent e in wave.SpawnStampedeEvents)
        {
            List<EnemyType> enemies = e.GetEnemiesInWave();
            for (int i = 0; i < enemies.Count; i++)
            {
                exp += Enemy.enemyData[enemies[i]].CalculateExperience(wave.waveNumber);
            }
        }
        EndLevel = wave.LevelFromExp(exp);
        EditorGUILayout.LabelField($"Starting Level: {StartingLevel} | End Level: {EndLevel}");
    }*/
}