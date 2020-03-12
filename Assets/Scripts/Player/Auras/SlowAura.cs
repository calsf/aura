using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Decrease move speed of player

public class SlowAura : MonoBehaviour
{
    PlayerMove playerMove;

    void OnDisable()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<AuraDefaults>().Player.GetComponent<PlayerMove>();
        }

        playerMove.Speed = (playerMove.BaseSpeed);
        playerMove.AirSpeed = (playerMove.BaseAirSpeed);
    }

    void OnEnable()
    {
        if (playerMove == null)
        {
            return;
        }

        playerMove.Speed = (playerMove.BaseSpeed / 1.5f);
        playerMove.AirSpeed = (playerMove.BaseAirSpeed / 1.5f);
    }
}
