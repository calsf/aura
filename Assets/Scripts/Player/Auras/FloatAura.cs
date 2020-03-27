using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows player to float by capping fall speed while active

public class FloatAura : MonoBehaviour
{
    PlayerMoveInput playerMove;
    float fallSpeed = -3f;

    void OnDisable()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<AuraDefaults>().Player.GetComponent<PlayerMoveInput>();
        }

        playerMove.MaxFallSpeed = playerMove.BaseMaxFallSpeed;
    }

    void OnEnable()
    {
        if (playerMove == null)
        {
            return;
        }

        playerMove.MaxFallSpeed = fallSpeed;
    }
}
