using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move back and forth between two points

public class MoveTwoPoints : StoppableMovementBehaviour
{
    EnemyDefaults enemyDefaults;

    [SerializeField]
    Transform posA;
    [SerializeField]
    Transform posB;

    Transform nextPos;

    [SerializeField]
    bool xFlip;
    [SerializeField]
    bool yFlip;

    bool stopMoving = false;

    void Start()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
        nextPos = posB;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        // Stop movement
        if (stopMoving)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, nextPos.position, enemyDefaults.MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextPos.position) <= 0.1f)
        {
            nextPos = nextPos != posA ? posA : posB;
        }

        //Swap facing x direction if necessary
        if (xFlip && (transform.localScale.x > 0 && transform.position.x < nextPos.position.x) || (transform.localScale.x < 0 && transform.position.x > nextPos.position.x))
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

        //Swap facing y direction if necessary
        if (yFlip && (transform.localScale.y > 0 && transform.position.y > nextPos.position.y) || (transform.localScale.y < 0 && transform.position.y < nextPos.position.y))
        {
            transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
        }
    }

    /* For other scripts to stop and resume movement for any other actions */
    // Stop moving
    public override void StopMoving()
    {
        stopMoving = true;
    }

    // Resume moving
    public override void ResumeMoving()
    {
        stopMoving = false;
    }

}
