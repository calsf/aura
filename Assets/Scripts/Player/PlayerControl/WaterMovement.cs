using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            playerMove.AccelAir = .25f;
            playerMove.AccelGrounded = .25f;

            playerMove.DefaultGrav = (2.5f * playerMove.Gravity);
            playerMove.LowGrav = (1.5f * playerMove.Gravity);
            playerMove.MaxFallSpeed = -15f;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
