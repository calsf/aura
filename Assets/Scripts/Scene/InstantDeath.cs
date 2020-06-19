using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If player gets hit before touching deathzone, player may be invulnerable when hitting deathzone
// InstantDeath will reset damaged time so player dies upon entering

public class InstantDeath : MonoBehaviour
{
    PlayerHP playerHP;

    void Awake()
    {
        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Reset damaged time so player can instantly take damage upon entering this collider
        if (other.tag == "PlayerDamaged")
        {
            playerHP.DamagedTime = 0;
        }
    }
}
