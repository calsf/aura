using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public string enemyName;
    public int maxHP;
    public int dmg;
    public int gold;
    public int baseMoveSpeed;
    public AudioClip deathSound;
    [Range(0, 1)]
    public float deathVolume;
    public GameObject deathFX;
    public AudioClip[] hitSounds; // 3 hit sounds
}
