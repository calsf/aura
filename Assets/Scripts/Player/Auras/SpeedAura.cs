using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Increase move speed of player

public class SpeedAura : MonoBehaviour
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

        playerMove.Speed = playerMove.BaseSpeed * 1.8f;
    }

}
