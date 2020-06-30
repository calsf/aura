using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoot upon entering collider, uses shoot fixed behaviour, should be handled by child object of enemy

public class ShootLoS : MonoBehaviour
{
    // Dynamic projectile pool
    List<GameObject> projectilePool;
    [SerializeField]
    int poolNum;

    float lastShot = -1;
    Animator anim;
    Transform spawnPos;     // Position of projectile spawn
    bool isShooting;

    [SerializeField]
    ShootFixedBehaviour shootBehaviour;

    void Awake()
    {
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
            projectilePool.Add(Instantiate(shootBehaviour.projectilePrefab, Vector3.down * 50, Quaternion.identity));
            projectilePool[i].SetActive(false);
        }
    }

    // Play Shoot animation to indicate shooting
    public void StartShoot()
    {
        anim.Play("Shoot"); // ANIMATION STATE MUST BE SAME NAME - ANIMATION CLIPS CAN BE DIFFERENT NAME
    }

    // Shoot projectile at player during animation (Called during/in animation itself)
    public void Shoot()
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

    // Reset shot
    public void StopShooting()
    {
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

    // Trigger shoot on entering collider
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerDamaged")
        {
            if (Time.time > lastShot && !isShooting)
            {
                isShooting = true;
                StartShoot();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
