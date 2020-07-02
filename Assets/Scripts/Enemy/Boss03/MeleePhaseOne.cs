using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePhaseOne : StateMachineBehaviour
{
    [SerializeField]
    float delay;
    float nextAttack;

    [SerializeField]
    float minY;
    [SerializeField]
    float maxY;

    Transform boss;
    EnemyDefaults enemyDefaults;
    GameObject player;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = animator.gameObject.transform;
        enemyDefaults = boss.GetComponent<EnemyDefaults>();

        nextAttack = Time.time + delay;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If can attack, go to animation to trigger dash behaviour
        if (Time.time > nextAttack)
        {
            animator.Play("MeleePhaseOneDashStartUp");
        }
        else
        {
            // Move to player's y position, cap at minY position
            float y = player.transform.position.y < minY ? minY : (player.transform.position.y > maxY ? maxY : player.transform.position.y);

            boss.transform.position =
                Vector3.MoveTowards(boss.transform.position, 
                new Vector2(boss.transform.position.x, y), enemyDefaults.MoveSpeed * Time.deltaTime);

            
        }
    }
}