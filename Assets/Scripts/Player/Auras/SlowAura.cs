using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Decrease move speed of player

public class SlowAura : MonoBehaviour
{
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
}
