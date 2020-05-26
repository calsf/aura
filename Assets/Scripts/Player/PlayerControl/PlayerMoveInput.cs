using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMoveInput : MonoBehaviour
{
    // Player's velocity
    Vector2 velocity;

    // Horizontal movement
    int move;   // Move input
    float xSmoothing;
    float accelGrounded = .1f;
    float accelAir = .15f;
    float baseSpeed = 10f;
    float speed;
    float lastSpeed;
    
    // Gravity
    float currGravity;
    float gravity = -9.81f;
    float defaultGrav = 3;
    float lowGrav = 2;

    // Jumping
    bool canJump;
    bool canDoubleJump;
    float baseJump = 15f;
    float jumpVelocity;

    // Max fall speed
    float baseMaxFallSpeed = -20f;
    float maxFallSpeed;

    // Dashing
    float dashDelay = .4f;
    float lastDash = 0f;
    float dashSpeed = 30f;
    float diagDashAngle = 25f;
    bool dashing;
    bool hasDashed;

    // Teleport
    bool isTeleport; // For teleporting to replace dash if teleport aura is activated
    int teleportUnits = 7;
    [SerializeField]
    LayerMask teleMask;
    [SerializeField]
    Animator teleportAuraAnim;
    [SerializeField]
    GameObject teleportFromPrefab;
    List<GameObject> telePool;
    [SerializeField]
    int poolNum;


    bool canInput = true;  // Used to disable movement inputs, keeps calling ApplyMovement - used for when player gets hit
    bool upJump;    // Check if Jump with Up setting is on, if so, jump when up is pressed
    bool axisUpDown;  // Treat axis input as a key down (for positive y axis)
    bool axisDownDown; // Treat axis input as a key down (for negative y axis)


    // UnityEvents for player actions
    public UnityEvent OnJump;
    public UnityEvent OnFall;
    public UnityEvent OnAirJump;
    public UnityEvent OnDash;
    public UnityEvent OnDashUp;
    public UnityEvent OnDashDiagUp;
    public UnityEvent OnDashDiagDown;

    // Getters and setters
    public float BaseSpeed { get { return baseSpeed; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float MaxFallSpeed { get { return maxFallSpeed; } set { maxFallSpeed = value; } }
    public float BaseMaxFallSpeed { get { return baseMaxFallSpeed; } set { baseMaxFallSpeed = value; } }
    public float JumpVelocity { get { return jumpVelocity; } set { jumpVelocity = value; } }
    public float BaseJump { get { return baseJump; } }
    public float Move { get { return move; } }
    public bool Dashing { get { return dashing; } }
    public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
    public bool CanInput { get { return canInput; } set { canInput = value; } }
    public float DefaultGrav { get { return defaultGrav; } }
    public bool IsTeleport { get { return isTeleport; } set { isTeleport = value; } }
    public float LastSpeed { get { return lastSpeed; } set { lastSpeed = value; } }

    //For OnControlChange
    public void UpdateControls()
    {
        upJump = PlayerPrefs.GetString("UpJump") == "On" ? true : false;
    }

    PlayerController controller;

    void Awake()
    {
        controller = GetComponent<PlayerController>();

        speed = baseSpeed;
        jumpVelocity = baseJump;
        maxFallSpeed = baseMaxFallSpeed;
        lastSpeed = speed;

        // Set gravity values
        lowGrav *= gravity;
        defaultGrav *= gravity;
        currGravity = defaultGrav;

        // Init teleport from pool
        telePool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            telePool.Add(Instantiate(teleportFromPrefab, Vector3.zero, Quaternion.identity));
            telePool[i].SetActive(false);
        }
    }

    void Update()
    {
        //Do not allow player input while in menus
        if (MenuManager.MenuInstance.IsMenu)
        {
            return;
        }

        // Disable player movement input if dashing or if cannot input
        if (!dashing && canInput)
        {
            Horizontal();
            Vertical();
            DashInput();
        }
        else if (!dashing && !canInput)
        {
            ApplyGravity(); // Keep applying gravity even if cannot input
        }

        ApplyMovement();
    }

    // Get dash input, dash movement handled by Dash coroutine
    void DashInput()
    {
        // If on ground, reset hasDashed
        if (controller.Collisions.below)
        {
            hasDashed = false;
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
                float x = Mathf.Cos(diagDashAngle * Mathf.Deg2Rad) * -dashSpeed;
                float y = Mathf.Sin(diagDashAngle * Mathf.Deg2Rad) * dashSpeed;
                StartCoroutine(Dash(x, y));
            }
            else if (inputs[0] && inputs[3])    // up right
            {
                float x = Mathf.Cos(diagDashAngle * Mathf.Deg2Rad) * dashSpeed;
                float y = Mathf.Sin(diagDashAngle * Mathf.Deg2Rad) * dashSpeed;
                StartCoroutine(Dash(x, y));
            }
            else if (inputs[2] && inputs[1])    // down left
            {
                float x = Mathf.Cos(diagDashAngle * Mathf.Deg2Rad) * -dashSpeed;
                float y = Mathf.Sin(diagDashAngle * Mathf.Deg2Rad) * -dashSpeed;
                StartCoroutine(Dash(x, y));
            }
            else if (inputs[2] && inputs[3])    //down right
            {
                float x = Mathf.Cos(diagDashAngle * Mathf.Deg2Rad) * dashSpeed;
                float y = Mathf.Sin(diagDashAngle * Mathf.Deg2Rad) * -dashSpeed;
                StartCoroutine(Dash(x, y));
            }
            else if (inputs[0])                 // up
            {
                StartCoroutine(Dash(0, dashSpeed - 5f));
            }
            else if (inputs[1])                 // left
            {
                // Default x and y
                float x = -dashSpeed;
                float y = 0;

                // Adjust dash x and y if on a slope
                if (controller.Collisions.climbingSlope)
                {
                    x = Mathf.Cos(diagDashAngle * Mathf.Deg2Rad) * -dashSpeed;
                    y = Mathf.Sin(25 * Mathf.Deg2Rad) * dashSpeed;
                }
                else if (controller.Collisions.descendingSlope)
                {
                    x = Mathf.Cos(diagDashAngle * Mathf.Deg2Rad) * -dashSpeed;
                    y = Mathf.Sin(diagDashAngle * Mathf.Deg2Rad) * -dashSpeed;
                }

                StartCoroutine(Dash(x, y)); ;
            }
            else if (inputs[3])                 // right
            {
                // Default x and y
                float x = dashSpeed;
                float y = 0;

                // Adjust dash x and y if on a slope
                if (controller.Collisions.climbingSlope)
                {
                    x = Mathf.Cos(diagDashAngle * Mathf.Deg2Rad) * dashSpeed;
                    y = Mathf.Sin(diagDashAngle * Mathf.Deg2Rad) * dashSpeed;
                }
                else if (controller.Collisions.descendingSlope)
                {
                    x = Mathf.Cos(diagDashAngle * Mathf.Deg2Rad) * dashSpeed;
                    y = Mathf.Sin(diagDashAngle * Mathf.Deg2Rad) * -dashSpeed;
                }

                StartCoroutine(Dash(x, y));
            }
        }
    }

    // Vertical movement input and velocity - Includes double jumping and fast falling
    void Vertical()
    {
        // Set velocity to 0 if hit ceiling or on ground
        if (controller.Collisions.above || controller.Collisions.below)
        {
            velocity.y = 0;

            // Reset jumps if on ground
            if (controller.Collisions.below)
            {
                canJump = true;
                canDoubleJump = true;
            }
        }


        // Can jump once off ground and can jump again in air if double jump hasn't been used
        if (( (canJump) || (canDoubleJump))
            && (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["JumpPad"])
            || (upJump && Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["UpButton"]) || (upJump && Input.GetAxisRaw("Vertical") == 1))))
        {
            // Treat axis input up as a KeyDown event
            if (!axisUpDown)
            {
                axisUpDown = true;
                OnJump.Invoke();    // OnJump event
                if (!controller.Collisions.below)    // Check if jumped in air
                {
                    OnAirJump.Invoke();
                }

                // If grounded, use jump, else if in air and can double jump, use the double jump
                if (canJump)
                {
                    canJump = false;
                }
                else if (canDoubleJump)
                {
                    canDoubleJump = false;
                }

                SoundManager.SoundInstance.PlaySound("Jump");

                // Apply jump velocity
                velocity.y = jumpVelocity;
            }
        }
        else
        {
            axisUpDown = false;   // Reset axisDown
        }

        // Hold for more jump height by lowering gravity, can only hold until player velocity starts descending
        if (velocity.y > 0 && (Input.GetKey(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKey(ControlsManager.ControlInstance.Padbinds["JumpPad"])
            || (upJump && Input.GetKey(ControlsManager.ControlInstance.Keybinds["UpButton"]) || (upJump && Input.GetAxisRaw("Vertical") == 1))))
        {
            currGravity = lowGrav; 
        }
        else
        {
            currGravity = defaultGrav;
        }

        // Fast fall instantly sets y velocity to max
        if ((!controller.Collisions.below && Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["DownButton"]) || Input.GetAxisRaw("Vertical") == -1) )  // Tap down to fast fall
        {
            if (!axisDownDown)
            {
                axisDownDown = true;
                currGravity = defaultGrav;
                velocity.y = maxFallSpeed;
            }
            else
            {
                ApplyGravity();
            }
        }
        else
        {
            ApplyGravity();
            axisDownDown = false;
        }
    }

    // Horizontal movement input and velocity
    void Horizontal()
    {
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

        // Set horizontal velocity based on input, smooth the velocity depending on if on ground or if in air
        float targetVelocityX = move * speed;
        if (Time.deltaTime != 0) // Avoid dividing by deltaTime 0
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref xSmoothing, (controller.Collisions.below ? accelGrounded : accelAir));
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref xSmoothing, (controller.Collisions.below ? accelGrounded : accelAir), Mathf.Infinity, .001f);
        }
        FlipX();
    }

    // Apply gravity
    void ApplyGravity()
    {
        // Apply gravity, do not go lower than maxFallSpeed y velocity
        velocity.y += currGravity * Time.deltaTime;
        if (velocity.y < maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
        }
    }

    // Apply velocity to player
    public void ApplyMovement()
    {
        // Apply velocity * Time.deltaTime
        controller.Move(velocity * Time.deltaTime);
    }

    // Dash movement, set dashing to true to temporarily disable normal movement to prevent overwriting dash velocity, also disable gravity
    // If using teleport aura, replace dash with teleport
    IEnumerator Dash(float x, float y)
    {
        dashing = true;
        velocity.x = x;
        velocity.y = y;
        FlipX();

        if (isTeleport)
        {
            // Replace dash with teleport, teleport distance determined by units
            int newX = 0;
            int newY = 0;
            if (x != 0 && y != 0)
            {
                newX = Mathf.Sign(x) > 0 ? teleportUnits - 2: -(teleportUnits - 2);
                newY = Mathf.Sign(y) > 0 ? teleportUnits - 2 : -(teleportUnits - 2);

                // Adjust teleport units on slopes
                if ((controller.Collisions.descendingSlope && Mathf.Sign(y) < 0) || (controller.Collisions.climbingSlope && Mathf.Sign(y) > 0))
                {
                    newX = Mathf.Sign(x) > 0 ? teleportUnits - 2 : -(teleportUnits - 2);
                    newY = Mathf.Sign(y) > 0 ? teleportUnits - 5 : -(teleportUnits - 5);
                }
            }
            else if (x != 0)
            {
                newX = Mathf.Sign(x) > 0 ? teleportUnits : -teleportUnits;
            }
            else if (y != 0)
            {
                newY = Mathf.Sign(y) > 0 ? (teleportUnits - 1) : -(teleportUnits - 1);
            }

            Teleport(newX, newY);
        }
        else
        {
            // Play dash sound
            SoundManager.SoundInstance.PlaySound("Dash");

            // Invoke proper dash event based on x and y velocity
            if ((velocity.x > 0 && velocity.y > 0) || (velocity.x < 0 && velocity.y > 0))
            {
                OnDashDiagUp.Invoke();
            }
            else if ((velocity.x > 0 && velocity.y < 0) || (velocity.x < 0 && velocity.y < 0))
            {
                OnDashDiagDown.Invoke();
            }
            else if (velocity.x == 0 && velocity.y > 0)
            {
                OnDashUp.Invoke();
            }
            else
            {
                OnDash.Invoke();
            }

            currGravity = 0;
            yield return new WaitForSeconds(.25f);

            hasDashed = true;    //Limit one dash in air, set after dash in case player dashed up from ground, if was grounded, dash will just reset

            // Reset values to give control back to player
            currGravity = defaultGrav;
            velocity.x = 0;
            velocity.y = 0;
            dashing = false;
        }
    }

    // Teleport to position, replaces dash command if isTeleport true
    public void Teleport(int x, int y)
    {
        // Set a teleportFrom object at player's starting position to show where they teleported from
        GameObject teleFrom = GetFromPool(telePool);
        teleFrom.transform.position = transform.position;
        teleFrom.SetActive(true);

        if ((velocity.x > 0 && velocity.y > 0) || (velocity.x < 0 && velocity.y > 0) || (velocity.x > 0 && velocity.y < 0) || (velocity.x < 0 && velocity.y < 0))   // Teleport diag
        {
            int adjustmentX = Mathf.Sign(x) > 0 ? 1 : -1;    // Check if teleporting left or right
            int adjustmentY = Mathf.Sign(y) > 0 ? 1 : -1;    // Check if teleporting up or down
            Vector2 newPos = new Vector2(transform.position.x + x, transform.position.y + y);

            // Check if can teleport to new position, if not, keep reducing the new position, will eventually reach player's original position if something is blocking
            RaycastHit2D hit = Physics2D.Raycast(newPos, Vector2.zero, 0, teleMask);
            while (hit.collider != null)
            {
                newPos = new Vector2(newPos.x - (adjustmentX), newPos.y - (adjustmentY));
                hit = Physics2D.Raycast(newPos, Vector2.zero, 0, teleMask);
            }

            transform.position = newPos;
        }
        else if (velocity.x == 0 && velocity.y > 0)     // Teleport up
        {
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y + y);

            // Check if can teleport to new position, if not, keep reducing the new position, will eventually reach player's original position if something is blocking
            RaycastHit2D hit = Physics2D.Raycast(newPos, Vector2.zero, 0, teleMask);
            while (hit.collider != null)
            {
                newPos = new Vector2(transform.position.x, newPos.y - 1);
                hit = Physics2D.Raycast(newPos, Vector2.zero, 0, teleMask);
            }

            transform.position = newPos;
        }
        else    // Teleport left and right
        {
            int adjustment = Mathf.Sign(x) > 0 ? 1 : -1;    // Check if teleporting left or right
            Vector2 newPos = new Vector2(transform.position.x + x, transform.position.y);

            // Check if can teleport to new position, if not, keep reducing the new position, will eventually reach player's original position if something is blocking
            RaycastHit2D hit = Physics2D.Raycast(newPos, Vector2.zero, 0, teleMask);
            while (hit.collider != null)
            {
                newPos = new Vector2(newPos.x - (adjustment), transform.position.y);
                hit = Physics2D.Raycast(newPos, Vector2.zero, 0, teleMask);
            }

            transform.position = newPos;
        }

        // Play teleport aura's default start animation
        teleportAuraAnim.Play("TeleportAuraStart");

        hasDashed = true;    //Limit one teleport in air

        // Reset values to give control back to player
        currGravity = defaultGrav;
        velocity.x = 0;
        velocity.y = 0;
        dashing = false;
    }

    //Get inactive object from pool
    GameObject GetFromPool(List<GameObject> pool)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }

        // If no object in the pool is available, create a new object and  add to the pool
        GameObject newObj = Instantiate(teleportFromPrefab, Vector3.zero, Quaternion.identity);
        telePool.Add(newObj);
        return newObj;
    }

    // Flips character facing direction
    void FlipX()
    {
        // Flip character as needed - do not flip if canInput is off
        if (canInput && (velocity.x > 0 && transform.localScale.x < 0 || velocity.x < 0 && transform.localScale.x > 0))
        {
            transform.localScale = (new Vector2(-transform.localScale.x, transform.localScale.y));
        }
    }

}
