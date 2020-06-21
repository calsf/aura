using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoMove : StateMachineBehaviour
{
    [SerializeField]
    float rightX;
    [SerializeField]
    float leftX;

    float nextX;
    Transform boss;
    EnemyDefaults enemyDefaults;

    float movedCount;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;
        boss = animator.gameObject.transform;
        enemyDefaults = boss.GetComponent<EnemyDefaults>();

        nextX = Random.Range(0, 2) == 1 ? rightX : leftX;
        movedCount = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Move to next x position
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, new Vector2(nextX, boss.transform.position.y),
            (enemyDefaults.MoveSpeed) * Time.deltaTime);

        // Move back and forth x positions
        if (Mathf.Abs(nextX - boss.transform.position.x) <= 0.1f)
        {
            nextX = nextX == rightX ? leftX : rightX;
            movedCount++;
        }

        if (movedCount > 2)
        {
            animator.Play("TornadoStateOff");
        }
    }
}
