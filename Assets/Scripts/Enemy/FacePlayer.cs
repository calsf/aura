using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stops enemy movement and faces player while the player is in view

public class FacePlayer : MonoBehaviour
{
    GameObject player;
    PlayerInView view;
    PlayerHP playerHP;

    // Subtracts value from the max distance between player and enemy that is considered to be in distance
    // Higher the value, the shorter the distance between player and enemy must be before chase behaviour is triggered
    // Max distance between player and enemy to trigger behaviour is approximately camera view by default
    [SerializeField]
    float xDistanceMinus;
    [SerializeField]
    float yDistanceMinus;

    StoppableMovementBehaviour movement;
    ShootBehaviour shootBehaviourScript;

    bool playerInView; // Must be in camera view and within distance to be in view (InView and InDistance)
    Animator anim;

    [SerializeField]
    bool moveWhileFacing;   // Either move while facing player or stop movement and only face player

     void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHP = player.GetComponent<PlayerHP>();
        view = player.GetComponent<PlayerInView>();

        movement = GetComponent<StoppableMovementBehaviour>();
        anim = GetComponent<Animator>();

        // Shoot behaviour must either be in main enemy object which this script is attached to or in child object
        shootBehaviourScript = GetComponentInChildren<ShootBehaviour>();
        // Do not shoot until aggro'd
        if (shootBehaviourScript != null)
        {
            shootBehaviourScript.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerInView = view.InView(transform) && view.InDistance(transform, xDistanceMinus, yDistanceMinus);

        // If player in view and not dead, stop moving and face player
        if (playerHP.CurrentHP > 0 && playerInView)
        {
            // If move while facing is false, stop moving and set animations to go to idle
            if (!moveWhileFacing)
            {
                anim.SetBool("StopMove", true);     // Need to set a trigger for animations to go into idle animation state
                movement.StopMoving();
            }

            // Activate any attached shooting behaviour while in view
            if (shootBehaviourScript != null && !shootBehaviourScript.enabled)
            {
                shootBehaviourScript.enabled = true;
            }

            //Swap facing x direction to the player
            if ((transform.localScale.x > 0 && transform.position.x < player.transform.position.x) 
                || (transform.localScale.x < 0 && transform.position.x > player.transform.position.x))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }

        }
        else
        {
            if (!moveWhileFacing)
            {
                anim.SetBool("StopMove", false);
            }

            // If not shooting, resume moving, if mid shoot animation, don't resume moving
            if (!moveWhileFacing && !anim.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
            {
                movement.ResumeMoving();
            }

            // Deactivate any attached shooting behaviour until player comes back into view
            if (shootBehaviourScript != null && shootBehaviourScript.enabled)
            {
                shootBehaviourScript.enabled = false;
            }
        }
    }
}
