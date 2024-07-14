using System;
using UnityEngine;

[Serializable]
public class IconList
{
    [Header("Stats")]
    public Sprite hpUp;
    public Sprite dmgUp;
    public Sprite critChanceUp;
    public Sprite expMultiUp;

    [Header("Rabi")]
    public Sprite moonlightDaggers;
    public Sprite carrotBarrage;
    public Sprite bunnyhop;
    public Sprite moonBeam;

    public static IconList instance;
}