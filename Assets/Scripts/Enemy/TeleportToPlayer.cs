using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Faces player when in view and periodically teleports to player

public class TeleportToPlayer : MonoBehaviour
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

    bool lastInView;
    bool playerInView; // Must be in camera view and within distance to be in view (InView and InDistance)
    Animator anim;

    // For teleporting to player
    [SerializeField]
    float minTeleDelay;
    [SerializeField]
    float maxTeleDelay;
    float nextTeleTime;
    bool isTeleporting;
    Vector2 teleportPos;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHP = player.GetComponent<PlayerHP>();
        view = player.GetComponent<PlayerInView>();

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInView = view.InView(transform) && view.InDistance(transform, xDistanceMinus, yDistanceMinus);

        // If player in view and not dead, stop moving and face player
        if (playerHP.CurrentHP > 0 && playerInView)
        {
            //Swap facing x direction to the player
            if ((transform.localScale.x > 0 && transform.position.x < player.transform.position.x)
                || (transform.localScale.x < 0 && transform.position.x > player.transform.position.x))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }

            // If player just came into view, first delay enemy teleport
            if (!lastInView)
            {
                nextTeleTime = Time.time + Random.Range(minTeleDelay, maxTeleDelay);
            }
            else if (Time.time > nextTeleTime && !isTeleporting)
            {
                isTeleporting = true;
                anim.Play("Teleport");
            }
        }

        lastInView = playerInView;
    }

    // Get player position and set it as position to teleport to
    void SetTelePosition()
    {
        teleportPos = player.transform.position;
    }

    // Teleport to assigned teleportPos
    void Teleport()
    {
        transform.position = teleportPos;
    }

    // Reset teleport
    void ResetTeleTime()
    {
        nextTeleTime = Time.time + maxTeleDelay;
        isTeleporting = false;
    }
}