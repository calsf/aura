using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    PlayerMoveInput move;
    PlayerController controller;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        move = GetComponent<PlayerMoveInput>();
        controller = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (move.Move == 0)
        {
            anim.SetFloat("Move", -1);  // Not moving
        }
        else
        {
            anim.SetFloat("Move", Mathf.Abs(move.Move));    // Get absolute value of move to see if moving or not
        }

        anim.SetFloat("yVelocity", move.Velocity.y);
        anim.SetBool("Grounded", controller.Collisions.below);
    }
}
