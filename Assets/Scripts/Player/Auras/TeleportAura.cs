using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Toggle teleport to replace dashing movement in PlayerMoveInput on aura activation

public class TeleportAura : MonoBehaviour
{
    PlayerMoveInput playerMove;

    void OnDisable()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<AuraDefaults>().Player.GetComponent<PlayerMoveInput>();
        }

        playerMove.IsTeleport = false;
    }

    void OnEnable()
    {
        if (playerMove == null)
        {
            return;
        }

        playerMove.IsTeleport = true;
    }
}
