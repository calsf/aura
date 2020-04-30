using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Slows both projectiles and enemies, to not overshadow the slow aura, the slow is less effective against living things

public class TemporalAura : MonoBehaviour
{
    float projectileSlowMultiplier = .3f;
    float restoreProjectileSpeedDelay = 1f;

    float enemySlowMultiplier = .6f;
    float restoreEnemySpeedDelay = .2f;

    // Reduce DamagePlayer speed
    void OnTriggerEnter2D(Collider2D other)
    {
        // Slow damage player objects such as projectiles
        if (other.tag == "DamagePlayer")
        {
            DamagePlayerDefaults dmgPlayer = other.GetComponent<DamagePlayerDefaults>();
            if (dmgPlayer != null)
            {
                dmgPlayer.RestoreMoveSpeedTime = Time.time + restoreProjectileSpeedDelay;
                dmgPlayer.Speed = dmgPlayer.DmgPlayer.baseSpeed * projectileSlowMultiplier;
            }
        }

        // Slow enemies
        if (other.tag == "Enemy")
        {
            EnemyDefaults enemy = other.GetComponent<EnemyDefaults>();
            if (enemy != null)
            {
                enemy.RestoreMoveSpeedTime = Time.time + restoreEnemySpeedDelay;
                enemy.MoveSpeed = enemy.Enemy.baseMoveSpeed * enemySlowMultiplier;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
