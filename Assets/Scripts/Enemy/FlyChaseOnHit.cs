using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Once enemy has lost health, it is aggro'd onto the player and will keep chasing the player, AGGRO IS NEVER RESET ONCE ENEMY HAS BEEN HIT UNLIKE FLYCHASEINVIEW
// Chase movement will ignore obstacles and go straight towards player
// If player is dead or out of enemy's view, then the enemy will activate nonChasingMove behaviour until player is alive/back in view but will remain aggro'd as soon as player comes into view

public class FlyChaseOnHit : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject player;
    PlayerInView view;
    PlayerHP playerHP;
    EnemyDefaults enemyDefaults;
    Animator anim;

    bool isStartingAggro;
    bool isAggro;

    // For non aggro movement after being hit
    [SerializeField]
    Transform posA;
    [SerializeField]
    Transform posB;
    Transform nextPos;
    [SerializeField]
    [Range(0, 1)]
    float nonChaseSpeedMultiplier;  // Multiply base enemy move speed to get speed while enemy is not chasing

    bool playerInView; // Must be in camera view and within distance to be in view (InView and InDistance)

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHP = player.GetComponent<PlayerHP>();
        view = player.GetComponent<PlayerInView>();

        nextPos = posB;
        enemyDefaults = GetComponent<EnemyDefaults>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        playerInView = view.InView(transform) && view.InDistance(transform);

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
        // If player is not in view, return
        if (!playerInView || playerHP.CurrentHP <= 0)
        {
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
            return;
        }
        else
        {
            // Chase player if aggro'd
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, enemyDefaults.MoveSpeed * Time.deltaTime);
        }
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
