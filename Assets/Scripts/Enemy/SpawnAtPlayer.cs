using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move spawn position to player's position

public class SpawnAtPlayer : MonoBehaviour
{
    GameObject player;
    Transform spawnPos;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

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
        spawnPos.position = new Vector2(player.transform.position.x, spawnPos.position.y);
    }
}
