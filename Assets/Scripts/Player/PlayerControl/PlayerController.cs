using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Raycasts
{
    [SerializeField]
    LayerMask collisionMask;
    float maxSlopeAngle = 70;

    // Store properties/info of player's current collisions
    CollisionInfo collisions;
    public CollisionInfo Collisions { get { return collisions; } }
    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public bool climbingSlope;
        public bool descendingSlope;

        public float slopeAngle;
        public float slopeAngleOld;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;

            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
    

    // Start
    public override void Start()
    {
        base.Start();
    }

    // Climb up slope
    void ClimbSlope(ref Vector2 velocity, float slopeAngle)
    {
        float dist = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * dist;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * dist * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    // Descend down slope
    void DescendSlope(ref Vector2 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);

        Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);   // Cast ray down

        // Check if raycast hits a downward slope and adjust velocity to descend down slope
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
            {
                if (Mathf.Sign(hit.normal.x) == dirX)
                {
                    if (hit.distance - offset <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float dist = Mathf.Abs(velocity.x);
                        float y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * dist;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * dist * Mathf.Sign(velocity.x);
                        velocity.y -= y;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    // Check horizontal raycast collisions
    void HorizontalCollisions(ref Vector2 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + offset;

        // Go through every raycast and check for hit
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);    // Cast ray in direction player is moving

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.red);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                // If the ground hit by raycast is an angle that can be climbed, climb the angle once close enough
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - offset;
                        velocity.x -= distanceToSlopeStart * dirX;
                    }

                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * dirX;
                }

                // Not climbing slope, set horizontal velocity based on collisions with left and right
                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    velocity.x = (hit.distance - offset) * dirX;
                    rayLength = hit.distance; // Adjust raycast lengths once hit

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = dirX == -1;
                    collisions.right = dirX == 1;
                }
            }
        }
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
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, collisionMask);   // Cast ray up if player is moving up, cast ray down if player is moving down

            Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);

            // Set vertical velocity based on collisions with above and below
            if (hit)
            {
                velocity.y = (hit.distance - offset) * dirY;
                rayLength = hit.distance;   // Adjust raycast lengths once hit

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = dirY == -1;
                collisions.above = dirY == 1;
            }
        }
    }

    // Move player after checking for collisions from raycasts and adjusting velocity as needed
    public void Move(Vector2 velocity, bool onPlatform = false)
    {
        // Update raycast origins and reset collisions
        UpdateRaycastOrigins();
        collisions.Reset();

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);

        // If on a platform, is colliding with ground
        if (onPlatform)
        {
            collisions.below = true;
        }
    }
}
