using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwo : StateMachineBehaviour
{
    [SerializeField]
    float delay;
    float nextAttack;

    EnemyDefaults enemyDefaults;
    GameObject player;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyDefaults = animator.gameObject.GetComponent<EnemyDefaults>();
        nextAttack = Time.time + delay;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If can attack, go to one of the anim clips which will trigger an attack behaviour
        if (Time.time > nextAttack)
        {
            animator.Play("SlashAttack");
        }
        else
        {
            animator.gameObject.transform.position = Vector3.MoveTowards(
                animator.gameObject.transform.position, player.transform.position, enemyDefaults.MoveSpeed * Time.deltaTime);
        }
    }
}
