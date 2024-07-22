using System;
using UnityEngine;

[Serializable]
public class AudioSounds
{
    [Header("UI")]
    public AudioClip ui_hover;
    public AudioClip ui_select;

    [Header("Player Sounds")]
    public AudioClip playerHurtSfx;
    public AudioClip playerLvlUpSfx;

    [Header("Enemy Sounds")]
    public AudioClip enemyHurtSound;
    public AudioClip enemyDeathSound;
    public AudioClip bossWalk;

    [Header("Object Sounds")]
    public AudioClip gemSound;
    public AudioClip coinSound;
    public AudioClip foodSound;

    [Header("Bullet Sounds")]
    public AudioClip grazeSound;
    public AudioClip superGrazeSound;

    [Header("Map Sounds")]
    public AudioClip warningWaveSound;
    public AudioClip warningBossWaveSound;
}