﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Moves between multiple points (more than 2 points)

public class MoveMultiplePoints : StoppableMovementBehaviour
{
    EnemyDefaults enemyDefaults;

    [SerializeField]
    Transform[] pos;    // All positions to move to

    [SerializeField]
    int startPos;       // Index of starting position

    int nextPos;

    bool stopMoving = false;

    [SerializeField]
    bool xFlip;

    // If isCycle TRUE then positions cycle from last point to first point
    // If isCycle FALSE then enemy reverses and goes back points
    [SerializeField]
    bool isCycle;   

    void Start()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();

        // Set starting position and move from there
        nextPos = startPos;
        transform.position = pos[nextPos].position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Stop movement
        if (stopMoving)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, pos[nextPos].position, enemyDefaults.MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, pos[nextPos].position) <= 0.1f)
        {
            // Position to move to is of index current position + 1, if nextPos is outside array, wrap back to beginning
            nextPos++;
            if (nextPos > pos.Length - 1)
            {
                nextPos = 0;

                //Swap facing x direction if necessary
                if (xFlip)
                {
                    transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                }

                // If not a cycle, reverse and go back
                if (!isCycle)
                {
                    Array.Reverse(pos);     // Reverse pos to go back
                }
            }
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

