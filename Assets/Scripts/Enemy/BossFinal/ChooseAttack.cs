﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseAttack : StateMachineBehaviour
{
    [SerializeField]
    float delay;
    float nextAttack;

    ActivateHoming homing;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        homing = animator.gameObject.GetComponent<ActivateHoming>();
        nextAttack = Time.time + delay;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If can attack, go to one of the anim clips which will trigger an attack behaviour
        if (Time.time > nextAttack)
        {
            // Shoot homing projectile unless it is active, if active, use teleport attack
            if (homing.HomingProjectile.activeInHierarchy)
            {
                animator.Play("TeleportAttack");
            }
            else
            {
                animator.Play("HomingAttack");
            }
        }
    }
}
