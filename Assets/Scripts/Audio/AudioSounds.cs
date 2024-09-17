using System;
using UnityEngine;

[Serializable]
public class AudioSounds
{
    [Header("UI")]
    public AudioClip ui_hover;
    public AudioClip ui_select;
    public AudioClip ui_dialogue_char;

    [Header("Player Sounds")]
    public AudioClip playerHurtSfx;
    public AudioClip playerLvlUpSfx;
    public AudioClip playerSpecialAvailableSfx;
    public AudioClip surpriseSfx;
    public AudioClip playerSpecialUseSfx;

    [Header("Enemy Sounds")]
    public AudioClip enemyHurtSound;
    public AudioClip enemyDeathSound;
    public AudioClip bossDeath;
    public AudioClip bossWalk;
    public AudioClip shootBullet;
    public AudioClip bossChargeAttack;
    public AudioClip bulletwaveShootSound;
    public AudioClip chargeBulletSound;
    public AudioClip bossPhaseEnd;

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

    [Header("Music")]
    public AudioClip stageComplete;
}