using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttack : StateMachineBehaviour
{
    [SerializeField]
    float x;
    float nextX;

    [SerializeField]
    float dashMultiplier;

    Transform boss;
    EnemyDefaults enemyDefaults;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;
        boss = animator.gameObject.transform;
        enemyDefaults = boss.GetComponent<EnemyDefaults>();

        // Move to position based on current facing direction
        nextX = boss.transform.localScale.x > 0 ? -x : x;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Move to target x position
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, new Vector2(nextX, boss.transform.position.y),
            (enemyDefaults.MoveSpeed * dashMultiplier) * Time.deltaTime);

        // Once reach target, rise back up
        if (Mathf.Abs(nextX- boss.transform.position.x) <= 0.1f)
        {
            animator.Play("ResetToTornadoStartUp");
        }

    }
}
