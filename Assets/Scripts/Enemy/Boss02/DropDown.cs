using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDown : StateMachineBehaviour
{
    [SerializeField]
    float fallMultiplier;
    [SerializeField]
    float groundY;

    [SerializeField]
    float rightX;
    [SerializeField]
    float leftX;

    Transform boss;
    EnemyDefaults enemyDefaults;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;
        boss = animator.gameObject.transform;
        enemyDefaults = boss.GetComponent<EnemyDefaults>();

        // Set facing direction to opposite of where enemy is
        boss.transform.localScale = new Vector2( (Mathf.Abs(rightX - boss.transform.position.x) <= 0.1f ? 1 : -1)
            , boss.transform.localScale.y);
 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Start falling down from current position
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, new Vector2(boss.transform.position.x, groundY),
            (enemyDefaults.MoveSpeed * fallMultiplier) * Time.deltaTime);

        // Once reach ground floor, play dash start up
        if (Mathf.Abs(groundY - boss.transform.position.y) <= 0.1f)
        {
            animator.Play("DashStartUp");
        }
    }
}
