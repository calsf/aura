using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAura : MonoBehaviour
{
    PlayerMove playerMove;

    void OnDisable()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<AuraDefaults>().Player.GetComponent<PlayerMove>();
        }

        playerMove.Jump = playerMove.BaseJump;
        playerMove.AirSpeed = playerMove.BaseAirSpeed;
    }

    void OnEnable()
    {
        if (playerMove == null)
        {
            return;
        }

        playerMove.Jump = playerMove.Jump * 1.3f;
        playerMove.AirSpeed = playerMove.BaseAirSpeed * 1.3f;
    }
}
