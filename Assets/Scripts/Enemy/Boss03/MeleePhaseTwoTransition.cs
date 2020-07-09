using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePhaseTwoTransition : StateMachineBehaviour
{
    [SerializeField]
    float targetX;

    Transform boss;
    EnemyDefaults enemyDefaults;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.gameObject.transform;
        enemyDefaults = boss.GetComponent<EnemyDefaults>();
        animator.SetBool("Dashing", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss.transform.position.x < targetX)  // First move to target X position
        {
            boss.transform.position =
                Vector3.MoveTowards(boss.transform.position,
                new Vector2(targetX, boss.transform.position.y), (enemyDefaults.MoveSpeed) * Time.deltaTime);
        }
        else
        {
            // Then transition to phase two
            animator.Play("MeleePhaseTwoStartUp");

            // Disables melee boss neck which is first child object of melee boss's parent object
            boss.parent.GetChild(0).gameObject.SetActive(false);
        }
    }
}