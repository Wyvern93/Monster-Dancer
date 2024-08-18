using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Audio/MapTrack", fileName = "MapTrack")]
[Serializable]
public class MapTrack : ScriptableObject
{
    public AudioClip music;
    public float tempo;
    public float offset;
    public float loopStart;
    public float loopEnd;
}