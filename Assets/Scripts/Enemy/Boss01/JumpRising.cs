using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRising : StateMachineBehaviour
{
    [SerializeField]
    float targetY;

    Transform boss;
    EnemyDefaults enemyDefaults;
    float accel;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;
        boss = animator.gameObject.transform;
        enemyDefaults = boss.GetComponent<EnemyDefaults>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Rise to target position
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, new Vector2(boss.transform.position.x, targetY), 
            (enemyDefaults.MoveSpeed + accel) * Time.deltaTime);

        // Once target reached, start falling
        if (boss.position.y >= targetY)
        {
            animator.Play("Falling");
        }
    }
}
