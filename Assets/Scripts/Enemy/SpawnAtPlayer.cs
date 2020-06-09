using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move spawn position to player's position

public class SpawnAtPlayer : MonoBehaviour
{
    GameObject player;
    PlayerMoveInput playerMove;
    Transform spawnPos;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMove = player.GetComponent<PlayerMoveInput>();

        foreach (Transform child in transform)
        {
            if (child.tag == "SpawnPos")
            {
                spawnPos = child;
            }
        }
    }

    public void SetPosition()
    {
        // Set slightly ahead of player position
        float offset = 0;
        if (playerMove.Move > 0)
        {
            offset = 1.5f;
        }
        else if (playerMove.Move < 0)
        {
            offset = -1.5f;
        }

        spawnPos.position = new Vector2(player.transform.position.x + offset, spawnPos.position.y);
    }
}
