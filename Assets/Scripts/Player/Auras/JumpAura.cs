using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAura : MonoBehaviour
{
    PlayerMoveInput playerMove;

    void OnDisable()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<AuraDefaults>().Player.GetComponent<PlayerMoveInput>();
        }

        playerMove.JumpVelocity = playerMove.BaseJump;
    }

    void OnEnable()
    {
        if (playerMove == null)
        {
            return;
        }

        playerMove.JumpVelocity = playerMove.JumpVelocity * 1.3f;
    }
}
