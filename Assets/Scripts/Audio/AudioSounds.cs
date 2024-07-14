using System;
using UnityEngine;

[Serializable]
public class AudioSounds
{
    [Header("Player Sounds")]
    public AudioClip playerHurtSfx;
    public AudioClip playerLvlUpSfx;

    [Header("Enemy Sounds")]
    public AudioClip enemyHurtSound;
    public AudioClip enemyDeathSound;
    public AudioClip bossWalk;

    [Header("Object Sounds")]
    public AudioClip gemSound;

    [Header("Bullet Sounds")]
    public AudioClip grazeSound;

    [Header("Map Sounds")]
    public AudioClip warningWaveSound;
    public AudioClip warningBossWaveSound;
}