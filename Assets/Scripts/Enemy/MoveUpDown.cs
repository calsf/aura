using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Object moves upwards and then falls back down - can also move in an x direction

public class MoveUpDown : MonoBehaviour
{
    EnemyDefaults enemyDefaults;
    Rigidbody2D rb;
    [SerializeField]
    float xMultiplier;  // Horizontal movement multiplier applied on enemyDefaults.MoveSpeed
    [SerializeField]
    float yMultiplier;  // Vertical movement multiplier applied on enemyDefaults.MoveSpeed
    [SerializeField]
    float decayRate;    // Rate at which y velocity decreases

    float yDecay;

    void Start()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float newY = (enemyDefaults.MoveSpeed * yMultiplier) - (yDecay += decayRate);

        // Cap falling velocity
        if (newY < -20)
        {
            newY = -20;
        }

        rb.velocity = new Vector2(enemyDefaults.MoveSpeed * xMultiplier, newY);
    }

    // If collides with ground layer, make the object start falling
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            yDecay = enemyDefaults.MoveSpeed * yMultiplier - decayRate;
        }
    }
}
