using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Movement:
 * Move side to side, air speed is slightly slower than ground speed 
 * Double jump, hold to jump slightly higher
 * Fast fall at any time while in air, fall speed is capped at the fast fall speed
 * 7-way directional dash, once in air, unlimited on ground, has delay between uses - dashing diagonally downwards and hitting the ground mid dash will cause player to slide
 * Dashing straight down is redundant with fast fall and is not included
 * 
 * Slope movement is handled with horizontal raycasts
 */

public class PlayerMove : MonoBehaviour {
    float move;
    float groundCheckRadius = .2f;
    float baseSpeed = 10f;
    float baseAirSpeed = 7f;
    float speed;
    float airSpeed;

    float lowGrav = 2f;
    float baseFallSpeed = -20f;
    float fallSpeed;
    float defaultGrav = 3f;
    float baseJump = 15f;
    float jump;
    
    float dashDelay = .4f;
    float lastDash = 0f;

    bool canJump;
    bool isGrounded;
    bool dashing;
    bool hasDashed;
    bool canDoubleJump;
    bool isFastFall;
    bool isJumping;

    bool upJump;    // Check if Jump with Up setting is on, if so, jump when up is pressed
    bool axisDown;  // Treat axis input as a key down

    LayerMask groundLayer;
    Rigidbody2D rb;

    [SerializeField]
    Transform groundChecker;

    public bool Grounded { get { return isGrounded; } }
    public float BaseSpeed { get { return baseSpeed; } }
    public float BaseAirSpeed { get { return baseAirSpeed; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float AirSpeed { get { return airSpeed; } set { airSpeed = value; } }
    public float FallSpeed { get { return fallSpeed; } set { fallSpeed = value; } }
    public float BaseFallSpeed { get { return baseFallSpeed; } set { baseFallSpeed = value; } }
    public float Jump { get { return jump; } set { jump = value; } }
    public float BaseJump { get { return baseJump; } }
    public float Move { get { return move; } }
    public bool Dashing { get { return dashing; } }

    // UnityEvents for player actions
    public UnityEvent OnJump;
    public UnityEvent OnFall;
    public UnityEvent OnAirJump;
    public UnityEvent OnDash;
    public UnityEvent OnDashUp;
    public UnityEvent OnDashDiagUp;
    public UnityEvent OnDashDiagDown;

    /********* Raycast Collision Detection ********/
    [SerializeField]
    LayerMask collisionMask;
    RaycastOrigins raycastOrigins;
    BoxCollider2D coll;
    const float width = .015f;
    int xRaycount = 3;
    float xSpacing;

    float maxSlopeClimbAngle = 45;
    float maxSlopeDownAngle = 45;

    struct RaycastOrigins
    {
        public Vector2 botLeft, botRight;
    }

    void UpdateRaycastOrigins()
    {
        // Set raycast origins at bounds of the box collider
        Bounds bounds = coll.bounds;
        bounds.Expand(width * -2);

        raycastOrigins.botLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.botRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    // Calculate spacing between raycasts at Start()
    void CalcSpacing()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(width * -2);

        xRaycount = Mathf.Clamp(xRaycount, 2, int.MaxValue);

        xSpacing = bounds.size.y / (xRaycount - 1);
    }

    // Update raycast origins then check for x collisions hit by raycasts and handle the collisions
    void CheckXCollisions(Vector3 velocity)
    {
        UpdateRaycastOrigins();

        CheckDownSlope(rb.velocity);
        CheckUpSlope(rb.velocity);
    }

    // Check if going up a slope and handle movement
    void CheckUpSlope (Vector3 velocity)
    {
        float dirX = Mathf.Sign(transform.localScale.x);    // Face raycast in direction player is facing
        float rayLength = Mathf.Abs(transform.localScale.x / transform.localScale.x) + width;

        for (int i = 0; i < xRaycount; i++)
        {
            Vector2 rayOrigin = dirX == -1 ? raycastOrigins.botLeft : raycastOrigins.botRight;
            rayOrigin += Vector2.up * (xSpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector3.right * dirX * rayLength, Color.red);

            if (hit)
            {
                // Check if hit is a slope based on angle of hit
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                // If hit is a slope, check if within range and climb
                if (i == 0 && slopeAngle <= maxSlopeClimbAngle)
                {
                    // Climb slope once within a certain range
                    if (hit.distance - width <= .1f)
                    {
                        isGrounded = true;
                        // Get x and y velocity based on the slope
                        float moveSpeed = dashing ? Mathf.Abs(rb.velocity.x) : Mathf.Abs(move * speed);
                        float x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveSpeed * Mathf.Sign(velocity.x);
                        float y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveSpeed;
                        Vector2 climbVelocity = new Vector2(x, y);

                        // If not jumping, is climbing and should apply the climb velocity
                        if (rb.velocity.y < jump)
                        {
                            // Treat being on slopes same as being grounded, isGrounded may reset to false when checking if the physics shape overlap touches ground so need to reset properties here
                            isGrounded = true;
                            ResetJump();
                            if (!dashing)
                            {
                                hasDashed = false;
                            }
                            Velocity(climbVelocity.x, climbVelocity.y);
                        }
                    }
                }
            }
        }
    }
   
    // Check if going down a slope and handle movement
    void CheckDownSlope (Vector3 velocity)
    {
        float dirX = Mathf.Sign(transform.localScale.x);
        Vector2 rayOrigin = dirX == -1 ? raycastOrigins.botRight : raycastOrigins.botLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxSlopeDownAngle)
            {
                if (Mathf.Sign(hit.normal.x) == dirX)
                {
                    if (hit.distance - width <= .1f)
                    {
                        isGrounded = true;

                        float moveSpeed = dashing ? Mathf.Abs(rb.velocity.x) : Mathf.Abs(move * speed);
                        float y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * -moveSpeed;
                        float x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveSpeed * Mathf.Sign(velocity.x);

                        if (rb.velocity.y < jump)
                        {
                            isGrounded = true;
                            ResetJump();
                            if (!dashing)
                            {
                                hasDashed = false;
                            }
                            Velocity(x, y);
                        }
                    }
                }
            }
        }
    }

    /********* End Raycast Collision Detection ********/



    // Use this for initialization
    void Awake () {
        UpdateControls();
        jump = baseJump;
        speed = baseSpeed;
        airSpeed = baseAirSpeed;
        fallSpeed = baseFallSpeed;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        groundLayer = LayerMask.GetMask("Ground");
	}

    void Start ()
    {
        CalcSpacing();
    }

    //For OnControlChange
    public void UpdateControls()
    {
        upJump = PlayerPrefs.GetString("UpJump") == "On" ? true : false;
    }

    // Update is called once per frame
    void Update () {
        //Do not allow player input while in menus
        if (MenuManager.MenuInstance.IsMenu)
        {
            return;
        }

        //Jump input
        if (!dashing && (canJump || canDoubleJump) &&
            (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["JumpPad"])
            || (upJump && (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["UpButton"]) || Input.GetAxisRaw("Vertical") == 1))))
        {
            // Treat axis input up as a KeyDown event
            if (!axisDown)
            {
                OnJump.Invoke();    // OnJump event
                if (!isGrounded)    // Check if jumped in air
                {
                    OnAirJump.Invoke();
                }
                isJumping = true;
                axisDown = true;
            }
        }
        else
        {
            axisDown = false; // Reset axisDown
        }

        //Horizontal movement input
        if (Input.GetKey(ControlsManager.ControlInstance.Keybinds["LeftButton"]) || Input.GetAxisRaw("Horizontal") == -1)
        {
            move = -1;   
        }
        else if (Input.GetKey(ControlsManager.ControlInstance.Keybinds["RightButton"]) || Input.GetAxisRaw("Horizontal") == 1)
        {
            move = 1;
        }
        else
        {
            move = 0;
        }

        //Dash command, if in air only one dash until grounded again
        bool[] inputs = {
            Input.GetKey(ControlsManager.ControlInstance.Keybinds["UpButton"]) || Input.GetAxisRaw("Vertical") == 1,
            Input.GetKey(ControlsManager.ControlInstance.Keybinds["LeftButton"]) || Input.GetAxisRaw("Horizontal") == -1,
            Input.GetKey(ControlsManager.ControlInstance.Keybinds["DownButton"]) || Input.GetAxisRaw("Vertical") == -1,
            Input.GetKey(ControlsManager.ControlInstance.Keybinds["RightButton"]) || Input.GetAxisRaw("Horizontal") == 1,
            Input.GetKey(ControlsManager.ControlInstance.Keybinds["DashButton"]) || Input.GetKey(ControlsManager.ControlInstance.Padbinds["DashPad"]) };
        if (inputs[4] && !dashing && !hasDashed && Time.time > lastDash)
        {
            lastDash = Time.time + dashDelay;
            

            // 0 = up, 1 = left, 2 = down, 3 = right * No dashing straight down, already fast falls
            if (inputs[0] && inputs[1])         // up left
            {
                StartCoroutine(Dash(-30, 20));
            }
            else if (inputs[0] && inputs[3])    // up right
            {
                StartCoroutine(Dash(30, 20));
            }
            else if (inputs[2] && inputs[1])    // down left
            {
                StartCoroutine(Dash(-30, -20));
            }
            else if (inputs[2] && inputs[3])    //down right
            {
                StartCoroutine(Dash(30, -20)); ;
            }
            else if (inputs[0])                 // up
            {
                StartCoroutine(Dash(0, 30));
            }
            else if (inputs[1])                 // left
            {
                StartCoroutine(Dash(-35, 0)); ;
            }
            else if (inputs[3])                 // right
            {
                StartCoroutine(Dash(35, 0));
            }
        }

        // Gravity
        if (!dashing && Input.GetKey(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKey(ControlsManager.ControlInstance.Padbinds["JumpPad"])
            || (upJump && ((Input.GetKey(ControlsManager.ControlInstance.Keybinds["UpButton"])) || Input.GetAxisRaw("Vertical") == 1))) // Hold space/jump to increase jump height
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = defaultGrav;
            }
            else
            {
                rb.gravityScale = lowGrav;
            }
            isFastFall = false;
        }
        else if (!dashing && !isGrounded && (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["DownButton"]) || Input.GetAxisRaw("Vertical") == -1))  // Tap down to fast fall
        {
            OnFall.Invoke();    // OnFall event
            rb.gravityScale = defaultGrav;
            isFastFall = true;
        }
        else
        {
            rb.gravityScale = defaultGrav;
        }
    }

    //Fixed Update
    void FixedUpdate()
    {
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, groundLayer);

        // Reset properties on grounded
        if (isGrounded)
        {
            ResetJump();
            hasDashed = false;
        }

        // Handle jumping/double jumping
        if (isJumping)
        {
            Velocity(0, jump);
            isGrounded = false;
            if (canJump)
            {
                canJump = false;
            }
            else
            {
                canDoubleJump = false;
            }
            isJumping = false;
        }

        // Move player horizontally
        if (!dashing && (move > 0 || move < 0))
        {
            // Flip character if necessary
            if (move > 0 && transform.localScale.x < 0) //If moving right and facing left, flip
            {
                transform.localScale = (new Vector2(-transform.localScale.x, transform.localScale.y));
            }
            else if (move < 0 && transform.localScale.x > 0) //If moving left and facing right, flip
            {
                transform.localScale = (new Vector2(-transform.localScale.x, transform.localScale.y));
            }

            // Adjust for air and ground movement speed
            if (isGrounded)  // Ground movement
            {
                Velocity(move * speed, rb.velocity.y);
            }
            else // Air movement
            {
                Velocity((move * airSpeed), rb.velocity.y);
            }
        }

        // If dashing downwards and hit ground, reset y velocity
        if (dashing && rb.velocity.y < 0 && isGrounded)
        {
            Velocity(rb.velocity.x, 0);
        }

        // Fall speed cap
        if (rb.velocity.y < fallSpeed)
        {
            Velocity(rb.velocity.x, fallSpeed);
        }

        // Fast fall
        if (isFastFall)
        {
            Velocity(rb.velocity.x, fallSpeed);
            isFastFall = false;
        }

        // Check for collisions by horizontal raycasts unless player is dashing, Dash coroutine will call the check so it can keep dash speed on slope the same
        if (!dashing)
        {
            CheckXCollisions(rb.velocity);
        }
    }


    // Change rigidbody velocity
    public void Velocity(float x, float y)
    {
        rb.velocity = new Vector2(x, y);
    }

    // Reset jumps
    public void ResetJump()
    {
        canJump = true;
        canDoubleJump = true;
    }

    // Dash movement,temporarily disable normal movement to prevent overriding dash velocity, also disable gravity
    IEnumerator Dash(float x, float y)
    {
        // Flip character if necessary
        if (x > 0 && transform.localScale.x < 0) //If moving right and facing left, flip
        {
            transform.localScale = (new Vector2(-transform.localScale.x, transform.localScale.y));
        }
        else if (x < 0 && transform.localScale.x > 0) //If moving left and facing right, flip
        {
            transform.localScale = (new Vector2(-transform.localScale.x, transform.localScale.y));
        }

        dashing = true;
        Velocity(x, y);
        CheckXCollisions(rb.velocity);  // Check if dashing on slopes
        x = rb.velocity.x;
        y = rb.velocity.y;

        // Invoke proper dash event based on x and y velocity
        if ((x > 0 && y > 0) || (x < 0 && y > 0))
        {
            OnDashDiagUp.Invoke();
        }
        else if ((x > 0 && y < 0) || (x < 0 && y < 0))
        {
            OnDashDiagDown.Invoke();
        }
        else if (x == 0 && y > 0)
        {
            OnDashUp.Invoke();
        }
        else
        {
            OnDash.Invoke();
        }

        rb.gravityScale = 0;
        yield return new WaitForSeconds(.25f);
        hasDashed = true;    //Limit one dash in air, set after dash in case player dashed up from ground, if was grounded, dash will just reset
        Velocity(0, 0);
        rb.gravityScale = defaultGrav;
        dashing = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundChecker.position, groundCheckRadius);
    }
}
