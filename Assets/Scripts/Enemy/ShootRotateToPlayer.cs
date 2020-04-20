using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoots at player if player is in camera view, requires an object that will rotate and face the player
// The object that will face player will also play animation indicating a projectile is being fired, the actual firing of the projectile
// will be handled during the animation using animation events
// Shoot delay and stopping to shoot or not stopping to shoot determined by shootBehaviour scriptable object

// ALWAYS ATTACHED TO CHILD OBJECT OF ENEMY SINCE OBJECT WILL ALWAYS ROTATE TO FACE PLAYER TO INDICATE THEY ARE BEING TARGETTED
// THIS WILL NEVER STOP ENEMY MOVEMENT SINCE IT WILL BE THE CHILD OBJECT THAT PLAYS SHOOT ANIMATION WHICH MEANS MAIN ENEMY ANIMATION WILL NEVER BE AFFECTED

public class ShootRotateToPlayer : ShootBehaviour
{
    // Dynamic projectile pool
    List<GameObject> projectilePool;
    [SerializeField]
    int poolNum;

    GameObject player;
    PlayerInView view;

    bool playerInView;      // Must be in camera view and within distance to be in view (InView and InDistance)
    Transform spawnPos;     // Position of projectile spawn
    bool facePlayer;        // Face player?
    Animator anim;  
    Vector3 shootPos;       // The position to shoot at

    bool wasAbove;
    bool wasBelow;

    float lastShot = -1;

    [SerializeField]
    ShootRotateToPlayerBehaviour shootBehaviour;

    void Awake()
    {
        facePlayer = true;
        player = GameObject.FindGameObjectWithTag("Player");
        view = player.GetComponent<PlayerInView>();
        anim = GetComponent<Animator>();
        spawnPos = transform.GetChild(0); // Spawn positon MUST BE FIRST CHILD OBJECT

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

    // Update is called once per frame
    void Update()
    {
        // The first time player comes into view (or the first time this object comes into player's camera view), reset lastShot by a random delay
        if (!playerInView && view.InView(transform) && view.InDistance(transform))
        {
            // Avoid rapid/irregular shots by going repeatedly going in and out of view
            if (Time.time > lastShot)
            {
                lastShot = Time.time + Random.Range(shootBehaviour.minShootDelay, shootBehaviour.maxShootDelay);
            }
        }
        playerInView = view.InView(transform) && view.InDistance(transform);

        // If above is restricted, means cannot turn and shoot player if player is above
        if (shootBehaviour.restrictAbove && player.transform.position.y > transform.position.y)
        {
            wasAbove = true;
            return;
        }
        else if (wasAbove)
        {
            // If was above, reset shooting behaviour
            wasAbove = false;
            if (Time.time > lastShot)
            {
                lastShot = Time.time + Random.Range(shootBehaviour.minShootDelay, shootBehaviour.maxShootDelay);
            }
        }

        // If below is restricted, means cannot turn and shoot player if player is below
        if (shootBehaviour.restrictBelow && player.transform.position.y < transform.position.y)
        {
            wasBelow = true;
            return;
        }
        else if (wasBelow)
        {
            // If was below, reset shooting behaviour
            wasBelow = false;
            if (Time.time > lastShot)
            {
                lastShot = Time.time + Random.Range(shootBehaviour.minShootDelay, shootBehaviour.maxShootDelay);
            }
        }

        // Rotate towards and face the player
        if (playerInView && facePlayer)
        {
            Vector3 target = player.transform.position - transform.position;
            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * shootBehaviour.turnSpeed);
        }

        // Shoot player
        if (playerInView && facePlayer && Time.time > lastShot)
        {
            StartShoot();
        }
    }

    // Play Shoot animation to indicate shooting, stop facing player to show where projectile is going to go
    public override void StartShoot()
    {
        anim.Play("Shoot"); // ANIMATION STATE MUST BE SAME NAME - ANIMATION CLIPS CAN BE DIFFERENT NAME
        facePlayer = false;

        // Get the enemy facing direction at time of StartShoot
        shootPos = transform.right.normalized;
    }

    // Shoot projectile at player during animation (Called during/in animation itself)
    public override void Shoot()
    {
        GameObject proj = GetFromPool(projectilePool);

        proj.transform.rotation = spawnPos.transform.rotation;
        proj.transform.position = spawnPos.transform.position;
        proj.SetActive(true);
        proj.GetComponent<Projectile>().SetDirection(shootPos);
    }

    // Reset facePlayer after shoot animation is finished (Called during/in animation itself)
    public override void StopShooting()
    {
        lastShot = Time.time + shootBehaviour.shootDelay;
        facePlayer = true;
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
