using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseAttack : StateMachineBehaviour
{
    [SerializeField]
    float delay;
    float nextAttack;

    [SerializeField]
    string[] animClips;
    int lastClip = -1;

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
            // Do not use same attack/anim clip as last one played, if homing projectile already active, do not use that attack
            int clip;
            do
            {
                clip = Random.Range(0, animClips.Length);
            } while (clip == lastClip && (homing.HomingProjectile.activeInHierarchy && animClips[clip] == "HomingAttack"));

            lastClip = clip;
            animator.Play(animClips[clip]);
        }
    }
}
