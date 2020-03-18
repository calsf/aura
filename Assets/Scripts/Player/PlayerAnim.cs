using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    PlayerMove move;
    Animator anim;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        move = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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

        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("Grounded", move.Grounded);
    }
}
