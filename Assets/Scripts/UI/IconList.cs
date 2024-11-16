using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class IconList
{
    public Sprite[] abilityAtlas;
    public Sprite[] reloadAtlas;
    public Sprite[] itemAtlas;
    [Header("Stats")]
    public Sprite hpUp;
    public Sprite dmgUp;
    public Sprite critChanceUp;
    public Sprite expMultiUp;
    public Sprite movSpeedUp;

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
    public Sprite Eclipse;
    public Sprite piercingShot;
    public Sprite boxofCarrots;
    public Sprite Riccochet;
    public Sprite NightSlash;

    public static IconList instance;


    public Sprite getAbilityIcon(string id)
    {
        return abilityAtlas.Single(s => s.name == id);
    }

    public Sprite getReloadIcon(string id)
    {
        return reloadAtlas.Single(s => s.name == id);
    }

    public Sprite getItemIcon(string id)
    {
        return itemAtlas.Single(s => s.name == id);
    }
}