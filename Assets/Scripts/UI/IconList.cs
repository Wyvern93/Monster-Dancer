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

    [Header("Bonus")]
    public Sprite coins;
    public Sprite heal;

    [Header("Rabi")]
    public Sprite moonlightDaggers;
    public Sprite carrotBarrage;
    public Sprite bunnyhop;
    public Sprite moonBeam;
    public Sprite orbitalMoon;
    public Sprite carrotJuice;
    public Sprite lunarPulse;
    public Sprite rabbitReflexes;
    public Sprite lunarRain;
    public Sprite illusionDash;
    public Sprite carrotDelivery;

    public static IconList instance;
}