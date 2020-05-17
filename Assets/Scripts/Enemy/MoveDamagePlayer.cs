﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves a DamagePlayer object between 2 points with a delay between moving positions

public class MoveDamagePlayer : MonoBehaviour
{
    DamagePlayerDefaults dmgPlayerDefaults;

    [SerializeField]
    Transform posA;
    [SerializeField]
    Transform posB;

    Transform nextPos;

    [SerializeField]
    float delay;
    float moveTime;

    [SerializeField]
    bool xFlip;
    [SerializeField]
    bool yFlip;

    bool stopMoving = false;

    void Start()
    {
        dmgPlayerDefaults = GetComponent<DamagePlayerDefaults>();
        nextPos = posB;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Stop movement
        if (stopMoving || moveTime > Time.time)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, nextPos.position, dmgPlayerDefaults.Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextPos.position) <= 0.1f)
        {
            nextPos = nextPos != posA ? posA : posB;
            moveTime = Time.time + delay;   // Once reaches position, add delay before moving again
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
}