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
    [SerializeField]
    Transform leftWallChecker;
    [SerializeField]
    Transform rightWallChecker;

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

    // UnityEvents for player actions
    public UnityEvent OnJump;
    public UnityEvent OnFall;

    // Use this for initialization
    void Awake () {
        UpdateControls();
        jump = baseJump;
        speed = baseSpeed;
        airSpeed = baseAirSpeed;
        fallSpeed = baseFallSpeed;
        rb = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Ground");
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
                StartCoroutine(Dash(-30, 0)); ;
            }
            else if (inputs[3])                 // right
            {
                StartCoroutine(Dash(30, 0));
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
            else  // Air movement
            {
                Velocity((move * airSpeed), rb.velocity.y);
            }
        }
        else
        {
            //Not moving
            //anim.SetFloat("move", -1);
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
        //anim.SetBool("dashing", true);

        dashing = true;
        Velocity(x, y);
        rb.gravityScale = 0;
        yield return new WaitForSeconds(.2f);
        hasDashed = true;    //Limit one dash in air, set after dash in case player dashed up from ground, if was grounded, dash will just reset
        Velocity(rb.velocity.x, 0);
        rb.gravityScale = defaultGrav;
        dashing = false;
        //anim.SetBool("dashing", false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundChecker.position, groundCheckRadius);
    }
}
