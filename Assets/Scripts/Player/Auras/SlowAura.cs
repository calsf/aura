using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Decrease move speed of player, also decrease movespeed of any enemies but decrease movespeed by more than player slow

public class SlowAura : MonoBehaviour
{
    float enemySlowMultiplier = .4f;
    PlayerMoveInput playerMove;

    void OnDisable()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<AuraDefaults>().Player.GetComponent<PlayerMoveInput>();
        }

        playerMove.Speed = playerMove.BaseSpeed;
    }

    void OnEnable()
    {
        if (playerMove == null)
        {
            return;
        }

        playerMove.Speed = playerMove.BaseSpeed / 2f;
    }

    // Slow enemy move speed
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            EnemyDefaults enemy = other.GetComponent<EnemyDefaults>();
            if (enemy != null)
            {
                enemy.MoveSpeed = enemy.Enemy.baseMoveSpeed * enemySlowMultiplier;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
