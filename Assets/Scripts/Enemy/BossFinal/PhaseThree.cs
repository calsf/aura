﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move to random position at double speed

public class PhaseThree : MonoBehaviour
{
    EnemyDefaults enemyDefaults;

    [SerializeField]
    Transform[] pos;    // All positions to move to

    int nextPos;

    bool startMoving;

    void Awake()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
    }

    void OnEnable()
    {
        startMoving = false;
    }

    void Update()
    {
        if (startMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos[nextPos].position, (enemyDefaults.MoveSpeed * 2) * Time.deltaTime);

            if (Vector3.Distance(transform.position, pos[nextPos].position) <= 0.1f)
            {
                int lastPos = nextPos;
                do
                {
                    nextPos = Random.Range(0, pos.Length);
                } while (lastPos == nextPos);
            }
        }
    }

    void FixedUpdate()
    {
        if (startMoving)
        {
            StartMoving();
        }
    }

    public void StartMoving()
    {
        startMoving = true;
    }
}