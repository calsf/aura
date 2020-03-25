using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move back and forth between two points

public class MoveTwoPoints : MonoBehaviour {
    EnemyDefaults enemyDefaults;

    [SerializeField]
    Transform posA;
    [SerializeField]
    Transform posB;

    Transform nextPos;
    float speed;

    [SerializeField]
    bool xFlip;
    [SerializeField]
    bool yFlip;

    void Start()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
        speed = enemyDefaults.MoveSpeed;   // Set movement based on moveSpeed in enemyDefaults
        nextPos = posB;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPos.position, speed * Time.deltaTime);

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
        if (yFlip && (transform.localScale.x > 0 && transform.position.y < nextPos.position.y) || (transform.localScale.y < 0 && transform.position.y > nextPos.position.y))
        {
            transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
        }
    }

}
