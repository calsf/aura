using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Once enemy has lost health, it is aggro'd onto the player and will keep chasing the player
// Chase movement will ignore obstacles and go straight towards player
// If player is dead or if aggroViewOnly, then the enemy will activate nonChasingMove behaviour until player is alive/back in view

public class FlyChaseOnHit : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject player;
    PlayerInView view;
    PlayerHP playerHP;
    EnemyDefaults enemyDefaults;
    Animator anim;
    MoveTwoPoints nonChasingMove;   // The movement of enemy if not chasing and has been aggro'd already
    bool isStartingAggro;
    bool isAggro;

    [SerializeField]
    bool aggroViewOnly;     // Should enemy chase only if in view?
    bool playerInView;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHP = player.GetComponent<PlayerHP>();
        view = player.GetComponent<PlayerInView>();

        nonChasingMove = GetComponent<MoveTwoPoints>();
        enemyDefaults = GetComponent<EnemyDefaults>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        playerInView = view.InView(transform);

        // Once lost HP, aggro onto player and chase them
        if (!isStartingAggro && enemyDefaults.HP < enemyDefaults.Enemy.maxHP)
        {
            isStartingAggro = true;
            StartAggro();
        }
    }

    void FixedUpdate()
    {
        if (isStartingAggro || isAggro)
        {
            //Swap facing x direction if necessary
            if ((transform.localScale.x > 0 && transform.position.x < player.transform.position.x) || (transform.localScale.x < 0 && transform.position.x > player.transform.position.x))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }

        // Return if enemy hasn't been aggro'd yet
        if (!isAggro)
        {
            return;
        }

        // Once aggro'd check if should be chasing or doing non chase movement instead
        // If player is not in view and only chases player if player is in view, return
        if ((aggroViewOnly && !playerInView) || playerHP.CurrentHP <= 0)
        {
            nonChasingMove.enabled = true;
            return;
        }
        else
        {
            nonChasingMove.enabled = false;
        }

        // Chase player if aggro'd
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, enemyDefaults.MoveSpeed * Time.deltaTime);
    }

    // Play aggro animation
    public void StartAggro()
    {
        anim.Play("StartAggro"); // ANIMATION STATE MUST BE SAME NAME - ANIMATION CLIPS CAN BE DIFFERENT NAME
    }

    // Turn isAggro on in animation
    public void ToggleAggro()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Enemy may have constraints on X,Y,Z if it is a still enemy so reset this on aggro - this is to prevent flock behavior from moving it
        isAggro = true;
    }
}
