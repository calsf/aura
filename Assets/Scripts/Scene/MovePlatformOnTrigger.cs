using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When player enters object, moves the assigned platform to index of position

public class MovePlatformOnTrigger : MonoBehaviour
{
    [SerializeField]
    MovingPlatform platform;
    [SerializeField]
    int posToMove;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            anim.SetBool("Triggered", true);
            
            // Reset progress if this is new position to move to and platform is currently stopped
            if (platform.Progress >= 1 && platform.NextPos != posToMove)
            {
                platform.Progress = 0;
            }

            platform.NextPos = posToMove;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        anim.SetBool("Triggered", false);
    }
}
