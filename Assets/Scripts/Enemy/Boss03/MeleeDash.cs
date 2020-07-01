using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDash : StateMachineBehaviour
{
    [SerializeField]
    float targetX;
    float startX;

    bool reachedTarget;

    [SerializeField]
    float dashMultiplier;

    Transform boss;
    EnemyDefaults enemyDefaults;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.gameObject.transform;
        enemyDefaults = boss.GetComponent<EnemyDefaults>();
        startX = boss.transform.position.x;

        reachedTarget = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!reachedTarget && boss.transform.position.x < targetX)  // First move to target X position
        {
            boss.transform.position =
                Vector3.MoveTowards(boss.transform.position,
                new Vector2(targetX, boss.transform.position.y), (enemyDefaults.MoveSpeed * dashMultiplier) * Time.deltaTime);
        }
        else // Then return back to start position
        {
            reachedTarget = true;

            if (boss.transform.position.x > startX)
            {
                boss.transform.position =
                    Vector3.MoveTowards(boss.transform.position,
                    new Vector2(startX, boss.transform.position.y), (enemyDefaults.MoveSpeed * dashMultiplier) * Time.deltaTime);
            }
            else
            {
                // Return back to animation state
                animator.Play("MeleePhaseOneAttack");
            }
        }
    }
}