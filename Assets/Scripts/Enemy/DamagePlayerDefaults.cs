using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For damaging objects that are not enemies such as obstacles, projectiles etc.

public class DamagePlayerDefaults : MonoBehaviour
{
    [SerializeField]
    DamagePlayer dmgPlayer;
    float restoreMoveSpeedTime = 0; // The time at which move speed should be restored
    float restoreRate;

    float speed;
    
    public int Dmg { get { return dmgPlayer.dmg; } }
    public DamagePlayer DmgPlayer { get { return dmgPlayer; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float RestoreMoveSpeedTime { get { return restoreMoveSpeedTime; } set { restoreMoveSpeedTime = value; } }

    void Awake()
    {
        restoreRate = dmgPlayer.baseSpeed / 50f; // Restore rate relative to base speed
        speed = dmgPlayer.baseSpeed;
    }

    void FixedUpdate()
    {
        // Gradually restore speed for a decaying slow effect from slows
        if (speed < dmgPlayer.baseSpeed && Time.time > restoreMoveSpeedTime)
        {
            speed += restoreRate;
            if (speed > dmgPlayer.baseSpeed)
            {
                speed = dmgPlayer.baseSpeed;
            }
        }
    }
}
