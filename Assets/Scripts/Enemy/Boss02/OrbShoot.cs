using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to shoot projectiles from an object that can rotate
// Projectile direction based on this object's rotation and relies on animation event to shoot
// Speed of animation can be increased and rotation can be increased but will steadily decay

public class OrbShoot : MonoBehaviour
{
    // Dynamic projectile pool
    List<GameObject> projectilePool;
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    int poolNum;

    // Projectile spawn positions, inv spawn pos should be spawn pos inverted
    [SerializeField]
    Transform spawnPos;
    [SerializeField]
    Transform invSpawnPos;

    // Rotation
    [SerializeField]
    float addRotAmount;
    float maxRot = 270;
    float rotation;
    
    // Animator speed
    [SerializeField]
    float speedUpAmount;
    float speed;
    float maxSpeed = 5;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            projectilePool.Add(Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity));
            projectilePool[i].SetActive(false);
        }
    }

    // Reset values when enabled
    void OnEnable()
    {
        rotation = 0;
        speed = 1;
    }

    public void Shoot()
    {
        GameObject proj = GetFromPool(projectilePool);
        proj.transform.position = spawnPos.position;
        proj.transform.right = transform.right;
        proj.SetActive(true);
        Projectile projProperties = proj.GetComponent<Projectile>();
        projProperties.Dir = proj.transform.right.normalized;

        // Shoot another projectile in opposite direction of proj
        GameObject projInv = GetFromPool(projectilePool);
        projInv.transform.position = invSpawnPos.position;
        projInv.transform.right = -transform.right;
        projInv.SetActive(true);
        Projectile projPropertiesInv = projInv.GetComponent<Projectile>();
        projPropertiesInv.Dir = projInv.transform.right.normalized;
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

    void FixedUpdate()
    {
        // Set rotation and animator speed
        transform.Rotate(0, 0, rotation);
        anim.speed = speed;

        // Decrease rotation and speed amount
        if (rotation > 0)
        {
            rotation -= .2f;
        }
        if (speed > 1)
        {
            speed -= .02f;
        }
        
        // Check rotation bounds
        if (rotation < 0)
        {
            rotation = 0;
        }
        else if (rotation > maxRot)
        {
            rotation = maxRot;
        }

        // Check speed bounds
        if (speed < 1)
        {
            speed = 1;
        }
        else if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }
    }

    // Add rotation amount, used OnDamaged event for an enemy
    public void AddRotation()
    {
        rotation += addRotAmount;
    }

    // Add speed amount, used OnDamaged event for an enemy
    public void SpeedUpShoot()
    {
        speed += speedUpAmount;
    }
}
