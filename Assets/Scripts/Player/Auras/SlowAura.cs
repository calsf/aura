﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Decrease move speed of player, also decrease movespeed of any enemies but decreases movespeed by more than player slow
// Reduces enemy move speed more than temporal aura

public class SlowAura : MonoBehaviour
{
    float enemySlowMultiplier = .3f;
    float restoreEnemySpeedDelay = .5f;

    PlayerMoveInput playerMove;

    void OnDisable()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<AuraDefaults>().Player.GetComponent<PlayerMoveInput>();
        }

        playerMove.Speed = playerMove.BaseSpeed;
        playerMove.LastSpeed = playerMove.Speed;
    }

    void OnEnable()
    {
        if (playerMove == null)
        {
            return;
        }

        playerMove.Speed = playerMove.BaseSpeed / 1.6f;
        playerMove.LastSpeed = playerMove.Speed;
    }

    // Slow enemy move speed
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            EnemyDefaults enemy = other.GetComponent<EnemyDefaults>();
            if (enemy != null)
            {
                enemy.RestoreMoveSpeedTime = Time.time + restoreEnemySpeedDelay;

                // Apply slow while also taking into consideration the move speed multiplier of enemy
                enemy.MoveSpeed = (enemy.Enemy.baseMoveSpeed * ((enemy.MoveSpeedMultiplier - 1) + enemySlowMultiplier)) ;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
