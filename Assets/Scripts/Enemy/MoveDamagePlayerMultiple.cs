using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves a damage player object between multiple points

public class MoveDamagePlayerMultiple : MonoBehaviour
{
    DamagePlayerDefaults dmgPlayer;

    [SerializeField]
    Transform[] pos;    // All positions to move to

    [SerializeField]
    int startPos;       // Index of starting position

    int nextPos;

    // If isCycle TRUE then positions cycle from last point to first point
    // If isCycle FALSE then reverses and goes back points
    [SerializeField]
    bool isCycle;

    void Start()
    {
        dmgPlayer = GetComponent<DamagePlayerDefaults>();

        // Set starting position and move from there
        nextPos = startPos;
        transform.position = pos[nextPos].position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, pos[nextPos].position, dmgPlayer.Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, pos[nextPos].position) <= 0.1f)
        {
            // Position to move to is of index current position + 1, if nextPos is outside array, wrap back to beginning
            nextPos++;
            if (nextPos > pos.Length - 1)
            {
                nextPos = 0;

                // If not a cycle, reverse and go back
                if (!isCycle)
                {
                    Array.Reverse(pos);     // Reverse pos to go back
                }
            }
        }
    }
}
