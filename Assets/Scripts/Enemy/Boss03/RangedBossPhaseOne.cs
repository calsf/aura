using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBossPhaseOne : MonoBehaviour
{
    // Dynamic projectile pool
    List<GameObject> projectilePool;
    [SerializeField]
    int poolNum;

    [SerializeField]
    GameObject spawnPos1;
    [SerializeField]
    GameObject spawnPos2;

    [SerializeField]
    ShootFixedBehaviour shootBehaviour1;
    [SerializeField]
    ShootFixedBehaviour shootBehaviour2;

    void Awake()
    {
        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            projectilePool.Add(Instantiate(shootBehaviour1.projectilePrefab, Vector3.down * 50, Quaternion.identity));
            projectilePool[i].SetActive(false);
        }
    }

    // Shoot using first shoot behaviour
    public void ShootFirst()
    {
        // Shoot from first spawn point
        for (int i = 0; i < shootBehaviour1.numProj; i++)
        {
            GameObject proj = GetFromPool(projectilePool);
            proj.transform.position = spawnPos1.transform.position;
            proj.SetActive(true);

            // Get and set projectile properties if exists
            Projectile projProperties = proj.GetComponent<Projectile>();
            if (projProperties == null)
            {
                return;
            }
            // Shoot projectiles in their fixed directions
            projProperties.Dir = (new Vector2(shootBehaviour1.xDirection[i], shootBehaviour1.yDirection[i]));
        }

        // Shoot from second spawn point
        for (int i = 0; i < shootBehaviour1.numProj; i++)
        {
            GameObject proj = GetFromPool(projectilePool);
            proj.transform.position = spawnPos2.transform.position;
            proj.SetActive(true);

            // Get and set projectile properties if exists
            Projectile projProperties = proj.GetComponent<Projectile>();
            if (projProperties == null)
            {
                return;
            }
            // Shoot projectiles in their fixed directions
            projProperties.Dir = (new Vector2(shootBehaviour1.xDirection[i], shootBehaviour1.yDirection[i]));
        }
    }

    // Shoot using second shoot behaviour
    public void ShootSecond()
    {
        // Shoot from first spawn point
        for (int i = 0; i < shootBehaviour1.numProj; i++)
        {
            GameObject proj = GetFromPool(projectilePool);
            proj.transform.position = spawnPos1.transform.position;
            proj.SetActive(true);

            // Get and set projectile properties if exists
            Projectile projProperties = proj.GetComponent<Projectile>();
            if (projProperties == null)
            {
                return;
            }
            // Shoot projectiles in their fixed directions
            projProperties.Dir = (new Vector2(shootBehaviour2.xDirection[i], shootBehaviour2.yDirection[i]));
        }

        // Shoot from second spawn point
        for (int i = 0; i < shootBehaviour1.numProj; i++)
        {
            GameObject proj = GetFromPool(projectilePool);
            proj.transform.position = spawnPos2.transform.position;
            proj.SetActive(true);

            // Get and set projectile properties if exists
            Projectile projProperties = proj.GetComponent<Projectile>();
            if (projProperties == null)
            {
                return;
            }
            // Shoot projectiles in their fixed directions
            projProperties.Dir = (new Vector2(shootBehaviour2.xDirection[i], -shootBehaviour2.yDirection[i]));
        }
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
        GameObject newProjectile = Instantiate(shootBehaviour1.projectilePrefab, Vector3.zero, Quaternion.identity);
        projectilePool.Add(newProjectile);
        return newProjectile;
    }



}
