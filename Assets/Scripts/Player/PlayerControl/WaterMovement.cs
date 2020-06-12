using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Affect player's movement when under water

public class WaterMovement : MonoBehaviour
{
    PlayerMoveInput playerMove;

    [SerializeField]
    SoundManager sound;
    bool wasFalling;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerMove = player.GetComponent<PlayerMoveInput>();
    }

    void Update()
    {
        // Play falling in water sound when player starts descending in water
        if (!wasFalling && playerMove.Velocity.y < -1)
        {
            sound.PlaySound("FallingInWater");
        }

        wasFalling = playerMove.Velocity.y < -1;
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
