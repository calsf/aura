using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Affect player's movement when under water

public class WaterMovement : MonoBehaviour
{
    PlayerMoveInput playerMove;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerMove = player.GetComponent<PlayerMoveInput>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerMove.CanDoubleJump = true;

            playerMove.AccelAir = .3f;
            playerMove.AccelGrounded = .25f;

            playerMove.DefaultGrav = (2f * playerMove.Gravity);
            playerMove.LowGrav = (1f * playerMove.Gravity);

            // Check for player's max fall speed so does not interfere with float aura
            if (playerMove.MaxFallSpeed < -15f)
            {
                playerMove.MaxFallSpeed = -15f;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
