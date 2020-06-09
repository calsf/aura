using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithGravity : Raycasts
{
    EnemyDefaults enemyDefaults;

    [SerializeField]
    Transform posA;
    [SerializeField]
    Transform posB;

    Transform nextPos;

    [SerializeField]
    bool xFlip;

    Vector2 velocity;

    // Gravity scale to determine how fast enemy falls
    [SerializeField]
    float gravityScale;

    float maxFallSpeed = -2.5f;
    float gravity;

    // Store properties/info of current collisions
    CollisionInfo collisions;
    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
        }
    }

    LayerMask collisionMask;
    public Vector2 Velocity { get { return velocity; } set { velocity = value; } }

    void Awake()
    {
        gravity = -1f * gravityScale;
        collisionMask = LayerMask.GetMask("Ground");

        enemyDefaults = GetComponent<EnemyDefaults>();
        nextPos = posB;
    }

    // Update is called once per frame
    void Update()
    {
        // Set velocity to direction enemy should move in, enemy move speed applied in ApplyMovement
        velocity.x = nextPos.position.x > transform.position.x ? 1 : -1;

        // Change next position once destination reached
        if (Mathf.Abs(transform.position.x - nextPos.position.x) <= 0.1f)
        {
            nextPos = nextPos != posA ? posA : posB;
        }

        //Swap facing x direction if necessary
        if (xFlip && (transform.localScale.x > 0 && transform.position.x < nextPos.position.x) || (transform.localScale.x < 0 && transform.position.x > nextPos.position.x))
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

        // Reset velocity if hits ground or ceiling
        if (collisions.above || collisions.below)
        {
            velocity.y = 0;
        }
        
        // Apply gravity force
        ApplyGravity();
 
        // Apply movement to enemy based on velocity * Time.deltaTime
        ApplyMovement(velocity * Time.deltaTime);
    }

    // Apply gravity
    void ApplyGravity()
    {
        // Apply gravity, do not go lower than maxFallSpeed y velocity
        // Amount of gravity to apply based on enemys current move speed
        velocity.y += (gravity * (enemyDefaults.MoveSpeed / enemyDefaults.Enemy.baseMoveSpeed)) * Time.deltaTime;

        if (velocity.y < maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
        }
    }

    // Apply velocity to enemy
    public void ApplyMovement(Vector2 velo)
    {
        velo *= enemyDefaults.MoveSpeed;
        // Update raycast origins
        UpdateRaycastOrigins();

        if (velocity.y != 0 || velocity.x != 0 || velocity.y < 0)   // Reset collisions only if needed, otherwise keep collisions info the same
        {
            collisions.Reset();
        }

        if (velo.y != 0)
        {
            VerticalCollisions(ref velo);
        }

        // Apply velocity which should be velocity * Time.deltaTime, is affected by enemy current move speed
        transform.Translate(velo);
    }

    // Check vertical raycast collisions
    void VerticalCollisions(ref Vector2 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + offset;

        // Go through every raycast and check for hit
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);

            // Set vertical velocity based on collisions with above and below
            if (hit)
            {
                velocity.y = (hit.distance - offset) * dirY;
                rayLength = hit.distance;   // Adjust raycast lengths once hit

                collisions.below = dirY == -1;
                collisions.above = dirY == 1;
            }
        }
    }
}
