﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move back and forth between two points

public class MoveLeftRight : MonoBehaviour {
    EnemyDefaults enemyDefaults;
    [SerializeField]
    Transform point1;
    [SerializeField]
    Transform point2;
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    bool moveLeft;
    [SerializeField]
    bool noFlip; //If false, can flip, if true, do not flip

    float xSpeed;

    void Start()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        xSpeed = enemyDefaults.MoveSpeed;   // Set movement based on moveSpeed in enemyDefaults

        if (moveLeft)
        {
            //Once reaching or past point 2, no longer move left
            if (transform.position.x <= point2.position.x)
            {
                moveLeft = false;
            }
            else
            {
                //Swap facing direction if necessary
                if (!noFlip && transform.localScale.x < 0)
                {
                    transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                }
                rb.velocity = new Vector2(-xSpeed, rb.velocity.y);
            }
        }
        else
        {
            if (transform.position.x >= point1.position.x)
            {
                moveLeft = true;
            }
            else
            {
                //Swap facing direction if necessary
                if (!noFlip && transform.localScale.x > 0)
                {
                    transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                }
                rb.velocity = new Vector2(xSpeed, rb.velocity.y);
            }
        }
	}

    //Set moveLeft
    public void SetMove(bool move)
    {
        moveLeft = move;
    }
}
