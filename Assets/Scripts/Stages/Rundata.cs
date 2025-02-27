using System;
using System.Collections.Generic;
using UnityEngine;

public class RunData
{
    public bool isInfinite;
    public int currentLoop = 0;
    public int stageMulti = 1;
    public GameObject characterPrefab;
    public string currentMap;
    public int coins;
    public Type ultimateChosen;

    public List<Enhancement> possibleStatEnhancements, possibleSkillEnhancements, possibleItemEnhancements;

    public RunData()
    {
        possibleStatEnhancements = new List<Enhancement>();
        possibleSkillEnhancements = new List<Enhancement>();
        possibleItemEnhancements = new List<Enhancement>();
    }

    // This basically removes any not obtained or obtainable enhancement in this run
    public void CalculatePossibleEnhancements()
    {

    }

    public void RemoveStatEnhancement(Enhancement enhancement)
    {
        possibleStatEnhancements.Remove(enhancement);
    }
    public void RemoveSkillEnhancement(Enhancement enhancement)
    {
        possibleSkillEnhancements.Remove(enhancement);
    }
    public void RemoveItemEnhancement(Enhancement enhancement)
    {
        possibleItemEnhancements.Remove(enhancement);
    }
}