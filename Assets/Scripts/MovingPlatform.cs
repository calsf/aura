using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Raycasts
{
    [SerializeField]
    LayerMask playerMask;   // LayerMask to detect raycast hits with player

    [SerializeField]
    Transform[] pos;        // Target positions of moving platform

    [SerializeField]
    float speed;
    [SerializeField] [Range(0,2)]
    float ease;             // Ease platform movement by value

    [SerializeField]
    float moveDelay;        // Delay moving to next position by moveDelay amount
    float nextMove;

    Vector2 move;
    int currPos = 0;
    float progress = 0;     // Progress for currPos to nextPos

    PlayerController controller;

    [SerializeField]
    bool isTriggerPlatform;

    int nextPos;

    public int NextPos { get { return nextPos; } set { nextPos = value; } }
    public float Progress { get { return progress; } set { progress = value; } }

    // Detect raycast hit and store calculated movement/info to apply to player
    List<PlayerMovement> playerMovement; 
    struct PlayerMovement
    {
        public Vector2 velocity;
        public bool onPlatform;     // If player is on platform
        public bool moveBefore;     // If player should be moved before platform is moved

        public PlayerMovement(Vector2 velocity, bool onPlatform, bool moveBefore)
        {
            this.velocity = velocity;
            this.onPlatform = onPlatform;
            this.moveBefore = moveBefore;
        }
    }

    void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        // Stops moving platform if is a trigger to move platform and has reached destination
        if (progress >= 1 && isTriggerPlatform)
        {
            return;
        }

        UpdateRaycastOrigins();

        // Calculate velocity of platform
        Vector2 velocity = CalculatePlatformMovement();

        // Calculate movement of player using platform's raycasts
        CalculatePlayerMovement(velocity);
        MovePlayer(true);   // Check if should move player before moving platform
        transform.Translate(velocity);
        MovePlayer(false); // Check if should move player after moving platform
    }

    // Ease platform movement
    float Ease(float x)
    {
        float val = ease + 1; // No easing when value is 1
        return Mathf.Pow(x, val) / (Mathf.Pow(x, val) + Mathf.Pow(1 - x, val));
    }

    // Calculate platform movement based on next position to move to
    Vector2 CalculatePlatformMovement()
    {
        // Do not move if moveDelay has not passed from last platform move
        if (Time.time < nextMove)
        {
            return Vector2.zero;
        }

        // Position to move to is of index current position + 1, if nextPos is outside array, wrap back to beginning
        if (!isTriggerPlatform)
        {
            nextPos = currPos + 1;
            if (nextPos > pos.Length - 1)
            {
                nextPos = 0;
            }
        }

        // Find distance between the two positions and calculate progress
        float distBetween = Vector2.Distance(pos[currPos].position, pos[nextPos].position);
        progress += Time.deltaTime * speed/distBetween;

        // Clamp the progress between 0 and 1 and apply easing to progress
        progress = Mathf.Clamp01(progress);
        float easedProgress = Ease(progress);

        Vector2 newPos = Vector2.Lerp(pos[currPos].position, pos[nextPos].position, easedProgress);

        // If is a move on trigger platform and reached destination, stop moving and set new curr pos to the nextPos
        if (progress >= 1 && isTriggerPlatform)
        {
            currPos = nextPos;
            return Vector2.zero;
        }

        // If not a trigger to move platform, move to next position
        // Has reached nextPos so reset progress and increment last position
        if (progress >= 1)
        {
            progress = 0;

            // If currPos is outside array, wrap back to beginning
            currPos++;
            if (currPos > pos.Length - 1)
            {
                currPos = 0;
            }

            // Move finished, delay next move
            nextMove = Time.time + moveDelay;
        }
        
        // Return the calculated newPos - platform's position to get velocity of platform
        return newPos - (Vector2) transform.position;
    }

    // Move player before or after platform
    void MovePlayer(bool moveBefore)
    {
        foreach (PlayerMovement p in playerMovement)
        {
            if (p.moveBefore == moveBefore)
            {
                // Move the player, if onPlatform then controller will set collisions below to true/is grounded
                controller.Move(p.velocity, p.onPlatform);
            }
        }
    }

    // Calculate the movement of player on platform
    void CalculatePlayerMovement(Vector2 velocity)
    {
        bool hasHit = false;
        playerMovement = new List<PlayerMovement>();

        // x and y direction of platform
        float dirX = Mathf.Sign(velocity.x);
        float dirY = Mathf.Sign(velocity.y);

        // Vertical movement, pushes player vertically
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + offset;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, playerMask);  // Cast rays up if moving up, cast rays down if moving down

                Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);

                if (hit && !hasHit)
                {
                    hasHit = true;
                    float x = dirY == 1 ? velocity.x : 0;
                    float y = velocity.y - (hit.distance - offset) * dirY;

                    // If raycast hit player and direction is 1, it means platform is moving up and player was hit by raycast, otherwise, player is below and not on platform
                    // Move player before platform since platform is moving up, do not want player to get stuck below platform
                    playerMovement.Add(new PlayerMovement(new Vector2(x, y), dirY == 1, true));
                }
            }
        }

        // Horizontal movement, pushes player horizontally
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + offset;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, playerMask);   // Cast ray in direction platform is moving

                Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.red);

                if (hit && !hasHit)
                {
                    hasHit = true;
                    float x = velocity.x - (hit.distance - offset) * dirX;
                    float y = -offset;

                    // Horizontal raycast hit means player is not on platform, move player before platform
                    playerMovement.Add(new PlayerMovement(new Vector2(x, y), false, true));
                }
            }
        }

        // Detect if player is ON TOP of a horizontal or downward moving platform and match the x and y velocity accordingly
        if (dirY == -1 || (velocity.y == 0 && velocity.x != 0))
        {
            float rayLength = offset * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, playerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * dirY, Color.red);

                if (hit && !hasHit)
                {
                    hasHit = true;
                    float x = velocity.x;
                    float y = velocity.y;

                    // Player is on top of platform, move platform first then move player since horizontal platform movement does not matter and downward must move platform first
                    playerMovement.Add(new PlayerMovement(new Vector2(x, y), true, false));
                }
            }
        }
    }
}
