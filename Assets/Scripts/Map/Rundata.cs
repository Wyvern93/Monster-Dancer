using System.Collections.Generic;
using UnityEngine;

public class RunData
{
    public bool isInfinite;
    public int currentLoop = 0;
    public GameObject characterPrefab;
    public string currentMap;

    public List<Enhancement> posibleEnhancements;

    public RunData()
    {
        posibleEnhancements = new List<Enhancement>();
    }

    // This basically removes any not obtained or obtainable enhancement in this run
    public void CalculatePossibleEnhancements()
    {

    }

    public void RemoveEnhancement(Enhancement enhancement)
    {
        posibleEnhancements.Remove(enhancement);
    }
}