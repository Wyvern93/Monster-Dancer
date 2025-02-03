using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cutscene : MonoBehaviour
{
    [SerializeReference] public List<CutsceneEvent> entries = new List<CutsceneEvent>();
}