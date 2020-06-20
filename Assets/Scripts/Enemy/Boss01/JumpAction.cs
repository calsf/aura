using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction : StateMachineBehaviour
{
    bool hasJumped;
    float delay = 1.5f;
    float nextJump;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Delay jump action by delay amount
        hasJumped = false;
        nextJump = Time.time + delay;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Start jump action
        if (!hasJumped && Time.time > nextJump)
        {
            hasJumped = true;
            animator.Play("Jump");
        }
    }
}
