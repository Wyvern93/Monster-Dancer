using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
[ExecuteAlways]
public class Wave : MonoBehaviour
{
    public int waveNumber;
    public List<SpawnCircleHordeEvent> SpawnStampedeEvents;

    public float LevelFromExp(int totalExp)
    {
        int currentExp = 0;
        int level = 1;

        while (totalExp >= currentExp)
        {
            totalExp -= currentExp;
            level++;
            currentExp = Player.CalculateExpCurve(level); // Use your provided formula
        }

        return level - 1; // Subtract 1 to get the correct level
    }
}