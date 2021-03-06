﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Walks back and forth, when player in view, plays startup animation to chase player then chases player until player is dead or out of view
// Chases along x and y axis / directly chases player
// Movement can be interrupted and stopped to shoot - min shoot delay must be greater than aggro start animation to prevent animation/behaviours conflicting

public class FlyChaseInView : StoppableMovementBehaviour
{
    GameObject player;
    PlayerInView view;
    PlayerHP playerHP;
    EnemyDefaults enemyDefaults;
    Animator anim;

    // For non aggro movement
    [SerializeField]
    Transform posA;
    [SerializeField]
    Transform posB;
    Transform nextPos;
    [SerializeField] [Range(0, 1)]
    float nonChaseSpeedMultiplier;  // Multiply base enemy move speed to get speed while enemy is not chasing

    // Subtracts value from the max distance between player and enemy that is considered to be in distance
    // Higher the value, the shorter the distance between player and enemy must be before chase behaviour is triggered
    // Max distance between player and enemy to trigger behaviour is approximately camera view by default
    [SerializeField]
    float xDistanceMinus;
    [SerializeField]
    float yDistanceMinus;

    bool isStartingAggro;
    bool isAggro;
    bool stopMoving;

    bool playerInView; // Must be in camera view and within distance to be in view (InView and InDistance)
    ShootBehaviour shootBehaviourScript;

    void Awake()
    {
        // Shoot behaviour must either be in main enemy object which this script is attached to or in child object
        shootBehaviourScript = GetComponentInChildren<ShootBehaviour>();
        // Do not shoot until aggro'd
        if (shootBehaviourScript != null)
        {
            shootBehaviourScript.enabled = false;
        }

        player = GameObject.FindGameObjectWithTag("Player");
        playerHP = player.GetComponent<PlayerHP>();
        view = player.GetComponent<PlayerInView>();

        enemyDefaults = GetComponent<EnemyDefaults>();
        anim = GetComponent<Animator>();

        nextPos = posB;
    }

    void Update()
    {
        playerInView = view.InView(transform) && view.InDistance(transform, xDistanceMinus, yDistanceMinus);

        // If player in view and not dead, start up aggro if haven't already
        if (playerHP.CurrentHP > 0 && playerInView && !isStartingAggro)
        {
            isStartingAggro = true;
            StartAggro();
        }
    }

    void FixedUpdate()
    {
        // Always face player when aggro or starting aggro
        if (isStartingAggro || isAggro)
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

        // Check if should be chasing or doing non chase movement instead
        // If player out of view, is dead, or is not aggro'd, move between points A and B at same speed as enemyDefaults.MoveSpeed or slower depending on the nonChaseSpeedMultiplier
        // Otherwise, enemy should be chasing player
        if (!playerInView || playerHP.CurrentHP <= 0 || (!isAggro && !isStartingAggro))
        {
            // Reset aggro properties and animation from aggro or starting aggro if was aggro/starting aggro (for example, if player went out of view during aggro/start aggro)
            if (isAggro || isStartingAggro)
            {
                anim.Play("NormalMovement");
                isStartingAggro = false;
                isAggro = false;

                // Do not shoot while not aggro'd
                if (shootBehaviourScript != null)
                {
                    shootBehaviourScript.enabled = false;
                }
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
        else
        {
            // Chase player if aggro'd
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, enemyDefaults.MoveSpeed * Time.deltaTime);
        }
    }

    // Play aggro animation
    // IF ENEMY ALSO HAS SHOOT BEHAVIOUR, THE MIN SHOOT DELAY MUST BE GREATER THAN THE STARTAGGRO ANIMATION OR THE SHOOT BEHAVIOUR ANIMATION MAY OVERRIDE
    public void StartAggro()
    {
        StopMoving();   // Stop movement for the aggro start up
        anim.Play("StartAggro"); // ANIMATION STATE MUST BE SAME NAME - ANIMATION CLIPS CAN BE DIFFERENT NAME
    }

    // Turn isAggro on in animation
    public void ToggleAggro()
    {
        ResumeMoving();     // Resume movement
        isAggro = true;

        // Activate shooting behaviour if it exists
        if (shootBehaviourScript != null)
        {
            shootBehaviourScript.enabled = true;
        }
    }

    /* For this script or other scripts to stop and resume movement for any other actions */
    // Stop moving
    public override void StopMoving()
    {
        stopMoving = true;
    }

    // Resume moving
    public override void ResumeMoving()
    {
        stopMoving = false;
    }
}
