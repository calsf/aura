using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoots at player if player is in camera view, requires an object that will rotate and face the player
// The object that will face player will also play animation indicating a projectile is being fired, the actual firing of the projectile
// will be handled during the animation using animation events

public class ShootPlayer : MonoBehaviour
{
    // Dynamic projectile pool
    List<GameObject> projectilePool;
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    int poolNum;

    GameObject player;
    PlayerInView view;

    bool playerInView;
    Transform spawnPos;     // Position of projectile spawn
    bool facePlayer;        // Face player?
    Animator anim;  
    Vector3 shootPos;       // The position to shoot at

    float lastShot;
    [SerializeField]
    float minShootDelay;
    [SerializeField]
    float maxShootDelay;

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
            projectilePool.Add(Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity));
            projectilePool[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // The first time player comes into view (or the first time this object comes into player's camera view), reset lastShot by a random delay
        if (!playerInView && view.InView(transform))
        {
            lastShot = Time.time + Random.Range(minShootDelay, maxShootDelay);
        }
        playerInView = view.InView(transform);

        // Face player
        if (playerInView && facePlayer)
        {
            transform.right = player.transform.position - transform.position;
        }

        // Shoot player
        if (playerInView && facePlayer && Time.time > lastShot)
        {
            StartShoot();
        }
    }

    // Play Shoot animation to indicate shooting, stop facing player to show where projectile is going to go
    public void StartShoot()
    {
        anim.Play("Shoot");
        facePlayer = false;

        // Get the player's position at time of StartShoot
        shootPos = (player.transform.position - spawnPos.transform.position).normalized;
    }

    // Shoot projectile at player during animation
    public void Shoot()
    {
        GameObject proj = GetFromPool(projectilePool);

        proj.transform.rotation = spawnPos.transform.rotation;
        proj.transform.position = spawnPos.transform.position;
        proj.SetActive(true);
        proj.GetComponent<Projectile>().SetDirection(shootPos);
    }

    // Reset facePlayer after shoot animation is finished
    public void StopShooting()
    {
        lastShot = Time.time + maxShootDelay;
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
        GameObject newProjectile = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
        projectilePool.Add(newProjectile);
        return newProjectile;
    } 

}
