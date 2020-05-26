using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Slows player's speed by the slow multiplier upon entering/staying inside collider, on exit, restores player's last speed

public class SlowTrigger : MonoBehaviour
{
    [SerializeField] [Range(0, 1)]
    float slowMultiplier;
    PlayerMoveInput player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveInput>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.Speed = player.LastSpeed * slowMultiplier;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

   void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.Speed = player.LastSpeed;
        }
    }
}
