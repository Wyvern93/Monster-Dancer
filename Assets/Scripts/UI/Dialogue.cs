using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Dialogue", fileName = "Dialogue")]
[Serializable]
public class Dialogue : ScriptableObject
{
    [SerializeReference] public List<CutsceneEvent> entries;
}