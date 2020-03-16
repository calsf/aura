using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Increase move speed of player

public class SpeedAura : MonoBehaviour
{
    PlayerMove playerMove;

    void OnDisable()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<AuraDefaults>().Player.GetComponent<PlayerMove>();
        }

        playerMove.Speed = playerMove.BaseSpeed;
        playerMove.AirSpeed = playerMove.BaseAirSpeed;
    }

    void OnEnable()
    {
        if (playerMove == null)
        {
            return;
        }

        playerMove.Speed = playerMove.BaseSpeed * 2f;
        playerMove.AirSpeed = playerMove.BaseAirSpeed * 2f;
    }

}
