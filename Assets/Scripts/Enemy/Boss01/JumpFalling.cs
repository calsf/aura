using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpFalling : StateMachineBehaviour
{
    [SerializeField]
    float groundY;
    [SerializeField]
    float[] targetsX;

    float chosenX;
    Transform boss;
    EnemyDefaults enemyDefaults;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;
        boss = animator.gameObject.transform;
        enemyDefaults = boss.GetComponent<EnemyDefaults>();

        // Choose x target position and set boss to the x position
        chosenX = targetsX[Random.Range(0, targetsX.Length)];
        boss.transform.position = new Vector2(chosenX, boss.transform.position.y);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Start falling down from current position
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, new Vector2(chosenX, groundY),
            (enemyDefaults.MoveSpeed * 1.5f) * Time.deltaTime);

        // Once reach ground floor, play landing animation
        if (Mathf.Abs(groundY - boss.transform.position.y) <= 0.1f)
        {
            animator.Play("Landing");
        }
    }
}