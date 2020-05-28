using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAtPlayer : Raycasts
{
    GameObject player;
    PlayerHP playerHP;
    EnemyDefaults enemyDefaults;
    Animator anim;

    // For non aggro movement
    [SerializeField]
    Transform posA;
    [SerializeField]
    Transform posB;
    Transform nextPos;
    [SerializeField]
    [Range(0, 1)]
    float nonChaseSpeedMultiplier;  // Multiply base enemy move speed to get speed while enemy is not chasing

    // aggroDist is distance player must be in to trigger dash attack
    [SerializeField]
    float aggroDist;
    bool playerInDist;

    bool isStartingAggro;
    bool isAggro;
    bool stopMoving;

    [SerializeField]
    float dashSpeedMultiplier;
    [SerializeField]
    float dashDuration;
    float dashStopTime;
    float xPos;
    bool playerInFront;

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

    void Awake()
    {
        collisionMask = LayerMask.GetMask("Ground");

        player = GameObject.FindGameObjectWithTag("Player");
        playerHP = player.GetComponent<PlayerHP>();

        enemyDefaults = GetComponent<EnemyDefaults>();
        anim = GetComponent<Animator>();

        nextPos = posB;
    }

    void Update()
    {
        playerInDist = Vector3.Distance(transform.position, player.transform.position) <= aggroDist;

        // If player in dash distance and not dead, start up aggro if haven't already
        if (playerHP.CurrentHP > 0 && !isStartingAggro && playerInDist)
        {
            isStartingAggro = true;
            StartAggro();
        }
    }

    void FixedUpdate()
    {
        // Always face player when starting aggro, once aggro'd enemy is dashing so do not face player
        if (isStartingAggro && !isAggro)
        {
            //Swap facing x direction to the player
            if ((transform.localScale.x > 0 && transform.position.x < player.transform.position.x) || (transform.localScale.x < 0 && transform.position.x > player.transform.position.x))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }

        // Stop movement here if needed so enemy still face player if currently aggro'd
        if (stopMoving)
        {
            return;
        }

        // If player out of distance and not aggro'd, is dead, or is not aggro'd, move between points A and B at same speed as enemyDefaults.MoveSpeed or slower depending on the nonChaseSpeedMultiplier
        // Otherwise, enemy should be dashing/moving towards player based on dashSpeedMultiplier
        if (!playerInDist && ((!isAggro && !isStartingAggro)) || playerHP.CurrentHP <= 0 || (!isAggro && !isStartingAggro))
        {
            // Reset aggro properties and animation from aggro or starting aggro if was aggro/starting aggro (for example, if player went out of view during aggro/start aggro)
            if (isAggro || isStartingAggro)
            {
                anim.Play("NormalMovement");
                isStartingAggro = false;
                isAggro = false;
            }

            // Move back and forth if not aggro'd
            transform.position = Vector3.MoveTowards(transform.position, nextPos.position, (enemyDefaults.MoveSpeed * nonChaseSpeedMultiplier) * Time.deltaTime);
            if (Vector3.Distance(transform.position, nextPos.position) <= 0.1f)
            {
                nextPos = nextPos != posA ? posA : posB;
            }

            //Swap facing x direction if necessary
            if ((transform.localScale.x > 0 && transform.position.x < nextPos.position.x) || (transform.localScale.x < 0 && transform.position.x > nextPos.position.x))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }
        else if (isAggro)
        {
            // Dash towards player as long as time has not reached dashStopTime or stop and reset aggro if over dash duration
            if (Time.time < dashStopTime)
            {
                // Check for collisions
                UpdateRaycastOrigins();
                collisions.Reset();
                HorizontalCollisions();

                // Reverse direction of dash if collided with a wall
                if (collisions.left)
                {
                    xPos = int.MaxValue;
                }
                else if (collisions.right)
                {
                    xPos = int.MinValue;
                }

                transform.position = 
                    Vector3.MoveTowards(transform.position, new Vector2(xPos, transform.position.y), (enemyDefaults.MoveSpeed * dashSpeedMultiplier) * Time.deltaTime);
            }
            else
            {
                isAggro = false;
                isStartingAggro = false;
                anim.Play("NormalMovement");
            }
        }
    }

    // Play aggro animation
    public void StartAggro()
    {
        StopMoving();   // Stop movement for the aggro start up
        anim.Play("StartAggro"); // ANIMATION STATE MUST BE SAME NAME - ANIMATION CLIPS CAN BE DIFFERENT NAME
    }

    // Turn isAggro on in animation and do dash attack, dash attack should reset aggro once finished
    public void DashAttack()
    {
        ResumeMoving();     // Resume movement
        isAggro = true;

        // Update whether or not the player is in front of enemy
        playerInFront = player.transform.position.x > transform.position.x;
        xPos = playerInFront ? int.MaxValue : int.MinValue;

        // Set time to stop dashing
        dashStopTime = Time.time + dashDuration;
    }

    /* For this script or other scripts to stop and resume movement for any other actions */
    // Stop moving
    public void StopMoving()
    {
        stopMoving = true;
    }

    // Resume moving
    public void ResumeMoving()
    {
        stopMoving = false;
    }


    // Check horizontal raycast collisions
    void HorizontalCollisions()
    {
        float dirX = Mathf.Sign(xPos);
        float rayLength = .5f + offset;

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
