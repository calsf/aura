using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Jump back and forth while player is not in view or dead
// Once player comes into view/distance, stop and play aggro animation
// Aggro animation will toggle the chase behaviour and enemy will chase player by jumping towards them

public class JumpChase : Raycasts
{
    EnemyDefaults enemyDefaults;
    Animator anim;

    GameObject player;
    PlayerInView view;
    PlayerHP playerHP;
    bool playerInView; // Must be in camera view and within distance to be in view (InView and InDistance)

    // Subtracts value from the max distance between player and enemy that is considered to be in distance
    // Higher the value, the shorter the distance between player and enemy must be before chase behaviour is triggered
    // Max distance between player and enemy to trigger behaviour is approximately camera view by default
    [SerializeField]
    float xDistanceMinus;
    [SerializeField]
    float yDistanceMinus;

    // Enemy velocity
    Vector2 velocity;

    // Multipliers used to calculate horizontal movement and jump velocity based on the enemy scriptable object's base move speed
    [SerializeField]
    float xMoveMultiplier;
    [SerializeField]
    float jumpVelocityMultiplier;

    // Gravity scale to determine how fast enemy falls
    [SerializeField]
    float gravityScale;
    
    float maxFallSpeed = -20f;
    float gravity;

    bool isStartingAggro;
    bool isAggro;

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
        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerHP = player.GetComponent<PlayerHP>();
        view = player.GetComponent<PlayerInView>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInView = view.InView(transform) && view.InDistance(transform, xDistanceMinus, yDistanceMinus);

        // If player in view and not dead, start up aggro if haven't already, waits until enemy is on ground so it doesn't start up mid jump
        if (playerHP.CurrentHP > 0 && playerInView && !isStartingAggro && collisions.below)
        {
            isStartingAggro = true;
            StartAggro();
        }
        else if (!playerInView || playerHP.CurrentHP <= 0) // Reset aggro properties if player dies or goes out of view
        {
            // Reset aggro properties and animation from aggro or starting aggro if was aggro/starting aggro (for example, if player went out of view during aggro/start aggro)
            if (isAggro || isStartingAggro)
            {
                isStartingAggro = false;
                isAggro = false;
            }
        }

        // Do not move while starting aggro
        if (isStartingAggro)
        {
            return;
        }

        // If aggro, adjust xMoveMultiplier sign so that enemy jump towards player, else jump back and forth
        if (isAggro && collisions.below)
        {
            if ((player.transform.position.x > transform.position.x && Mathf.Sign(xMoveMultiplier) < 0) 
                || (player.transform.position.x < transform.position.x && Mathf.Sign(xMoveMultiplier) > 0))
            {
                xMoveMultiplier *= -1;
            }
        }
        else if (collisions.below)
        {
             xMoveMultiplier *= -1;
        }

        // Face direction enemy is jumping/moving in
        transform.localScale = new Vector2(-Mathf.Sign(xMoveMultiplier), transform.localScale.y);

        // Reset velocities if hits ground or ceiling
        if (collisions.above || collisions.below)
        {
            velocity.y = 0;
            velocity.x = 0;

            // Apply jump velocity if enemy is touching ground
            if (collisions.below)
            {
                velocity.y = jumpVelocityMultiplier;
                velocity.x = xMoveMultiplier;
            }
        }

        // Apply gravity force
        ApplyGravity();

        // Apply movement to enemy based on velocity * Time.deltaTime
        ApplyMovement(velocity * Time.deltaTime);
        
    }

    // Play aggro animation
    public void StartAggro()
    {
        anim.Play("StartAggro"); // ANIMATION STATE MUST BE SAME NAME - ANIMATION CLIPS CAN BE DIFFERENT NAME
    }

    // Turn isAggro on in animation
    public void ToggleAggro()
    {
        isAggro = true;
        isStartingAggro = false;
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
    public void ApplyMovement(Vector2 velocity)
    {
        // Update raycast origins
        UpdateRaycastOrigins();

        if (velocity.y != 0 || velocity.x != 0 || velocity.y < 0)   // Reset collisions only if needed, otherwise keep collisions info the same
        {
            collisions.Reset();
        }

        // Check for horizontal and vertical collisions
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        // Set animator values based on y velocity
        //anim.SetFloat("yVelocity", Mathf.Sign(velocity.y));

        // Apply velocity which should be velocity * Time.deltaTime, is affected by enemy current move speed
        transform.Translate(velocity * enemyDefaults.MoveSpeed);
    }

    // Check vertical raycast collisions
    void VerticalCollisions(ref Vector2 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + offset ;

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
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);    // Cast ray in direction object is moving

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.red);

            if (hit)
            {
                collisions.left = dirX == -1;
                collisions.right = dirX == 1;
            }
        }
    }

}
