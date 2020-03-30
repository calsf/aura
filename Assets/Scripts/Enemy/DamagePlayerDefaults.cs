using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For damaging objects that are not enemies such as obstacles, projectiles etc.

public class DamagePlayerDefaults : MonoBehaviour
{
    [SerializeField]
    DamagePlayer dmgPlayer;

    float speed;

    public int Dmg { get { return dmgPlayer.dmg; } }
    public float Speed { get { return speed; } set { speed = value; } }

    void Awake()
    {
        speed = dmgPlayer.baseSpeed;
    }
}
