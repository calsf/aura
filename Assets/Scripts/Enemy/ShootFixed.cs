using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoots projectiles in fixed directions, can also shoot in direction enemy is facing depending on shootBehaviour
// THIS CAN STOP ENEMY MOVEMENT TO STOP ONLY IF IT IS ATTACHED TO MAIN ENEMY OBJECT

// Stop to play animation to shoot -> attach to main enemy since enemy moving animation will be interrupted to play animation to shoot
// Do not stop to play animation to shoot -> attach to child of enemy object since child can play animation without interrupting main enemy move animation
// BUT can still attach to main enemy if animation to shoot transitions well from the move animation
// If enemy has no movement, can attach to main enemy or enemy children

public class ShootFixed : ShootBehaviour
{
    // Dynamic projectile pool
    List<GameObject> projectilePool;
    [SerializeField]
    int poolNum;

    PlayerInView view;      
    bool playerInView;      // Must be at camera edge view to be in view (EdgeOfView)
    float lastShot = -1;
    Transform spawnPos;     // Position of projectile spawn
    Animator anim;
    bool isShooting;

    StoppableMovementBehaviour[] movementBehaviours;

    [SerializeField]
    ShootFixedBehaviour shootBehaviour;

    void Awake()
    {
        // Get all movement behaviours enemy has so that they can be stopped and resumed 
        // IF A CHILD OBJECT OF ENEMY IS HANDLING THE SHOOTING, THERE SHOULD BE NO NEED TO STOP TO SHOOT
        // THE ANIMATION TO BE PLAYED TO INDICATE SHOOTING IS SEPARATE FROM MAIN ENEMY OBJECT AND SO ENEMY CAN SHOOT AND MOVE AT SAME TIME
        // STOP TO SHOOT SHOULD ONLY BE IF ENEMY ITSELF IS HANDLING THE SHOOTING WHICH WOULD NORMALLY REQUIRE ENEMY TO STOP MOVING AND PLAY THE SHOOTING ANIMATION
        movementBehaviours = GetComponents<StoppableMovementBehaviour>();

        view = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInView>();
        anim = GetComponent<Animator>();

        foreach (Transform child in transform)
        {
            if (child.tag == "SpawnPos")
            {
                spawnPos = child;
            }
        }

        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            projectilePool.Add(Instantiate(shootBehaviour.projectilePrefab, Vector3.zero, Quaternion.identity));
            projectilePool[i].SetActive(false);
        }
    }

    // Don't shoot immediately on enable
    void OnEnable()
    {
        // Avoid rapid/irregular shots by going repeatedly going in and out of view
        if (Time.time > lastShot)
        {
            lastShot = Time.time + Random.Range(shootBehaviour.minShootDelay, shootBehaviour.maxShootDelay);
        }
    }

    void Update()
    {
        // The first time player comes almost into view (or the first time this object comes into player's camera view), reset lastShot by a random delay
        // Checks for player ALMOST in view so player may be right at edge of this enemy's view and will start shooting
        if (!playerInView && view.EdgeOfView(transform))
        {
            // Avoid rapid/irregular shots by going repeatedly going in and out of view
            if (Time.time > lastShot)
            {
                lastShot = Time.time + Random.Range(shootBehaviour.minShootDelay, shootBehaviour.maxShootDelay);
            }
        }
        playerInView = view.EdgeOfView(transform);

        if (playerInView && !isShooting && Time.time > lastShot)
        {
            isShooting = true;
            StartShoot();
        }
    }

    // Play Shoot animation to indicate shooting
    public override void StartShoot()
    {
        // If behaviour stops to shoot, stop movement to shoot
        if (shootBehaviour.stopToShoot)
        {
            foreach (StoppableMovementBehaviour m in movementBehaviours)
            {
                m.StopMoving();
            }
        }

        anim.Play("Shoot"); // ANIMATION STATE MUST BE SAME NAME - ANIMATION CLIPS CAN BE DIFFERENT NAME
    }

    // Shoot projectile at player during animation (Called during/in animation itself)
    public override void Shoot()
    {
        for (int i = 0; i < shootBehaviour.numProj; i++)
        {
            GameObject proj = GetFromPool(projectilePool);
            proj.transform.position = spawnPos.transform.position;
            proj.SetActive(true);

            Projectile projProperties = proj.GetComponent<Projectile>();

            // Shoot in direction enemy is facing or shoot according to directions of each projectile
            if (shootBehaviour.shootFacingDirection && shootBehaviour.xDirection[i] != 0)
            {
                // Change xDirection to enemy's facing direction - WILL NOT WORK UNLESS ENEMY FLIPS ITS LOCALSCALE PROPERLY, ENEMY MUST ORIGINALLY FACE TO THE LEFT WHEN LOCALSCALE IS 1
                int xDir = -1;

                // Make sure to shoot in direction the main enemy object is facing, child objects of main enemy will not change their facing direction/localScale value
                if ((shootBehaviour.shootIsChild && transform.parent.transform.localScale.x < 0) || (!shootBehaviour.shootIsChild && transform.localScale.x < 0))
                {
                    xDir = 1;
                }

                projProperties.Dir = (new Vector2(xDir, shootBehaviour.yDirection[i]));   // Keep y direction of the projectile
            }
            else
            {
                // Shoot projectiles in their fixed directions
                projProperties.Dir = (new Vector2(shootBehaviour.xDirection[i], shootBehaviour.yDirection[i]));
            }
        }
    }

    // Reset facePlayer after shoot animation is finished (Called during/in animation itself)
    public override void StopShooting()
    {
        // If behaviour stops to shoot, resume movement after shooting
        if (shootBehaviour.stopToShoot)
        {
            foreach (StoppableMovementBehaviour m in movementBehaviours)
            {
                m.ResumeMoving();
            }
        }

        isShooting = false;
        lastShot = Time.time + shootBehaviour.shootDelay;
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
        GameObject newProjectile = Instantiate(shootBehaviour.projectilePrefab, Vector3.zero, Quaternion.identity);
        projectilePool.Add(newProjectile);
        return newProjectile;
    }

}
