using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePhaseTwo : MonoBehaviour
{
    EnemyDefaults enemyDefaults;

    // All positions to move to
    // Bot Left -> Bot Right -> Top Right -> Top Left
    // Set top left equal to player's x position
    [SerializeField]
    Transform[] pos;    

    [SerializeField]
    int startPos;       // Index of starting position

    int nextPos;
    
    [SerializeField]
    float moveDelay;
    float nextMove;

    Transform player;

    public float MoveDelay { get { return moveDelay; } set { moveDelay = value; } }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyDefaults = GetComponent<EnemyDefaults>();

        // Set starting position and move from there
        nextPos = startPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Do not move if moveDelay has not passed from last move
        if (Time.time < nextMove)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, pos[nextPos].position, enemyDefaults.MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, pos[nextPos].position) <= 0.1f)
        {
            // Position to move to is of index current position + 1, if nextPos is outside array, wrap back to beginning
            nextPos++;

            if (nextPos == pos.Length - 1)
            {
                // Set top left and bottom left x position equal to player's current position
                float x = player.transform.position.x;
                pos[pos.Length - 1].position = new Vector2(x, pos[pos.Length - 1].position.y);
                pos[0].position = new Vector2(x, pos[0].position.y);
            }
            if (nextPos > pos.Length - 1)
            {
                nextPos = 0;
            }

            // Move finished, delay next move
            nextMove = Time.time + moveDelay;
        }
    }
}
