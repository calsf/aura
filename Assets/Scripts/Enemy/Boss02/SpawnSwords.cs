using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnSwords : MonoBehaviour
{
    // Dynamic projectile pool
    List<GameObject> projectilePool;
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    int poolNum;

    // Possible x and y coordinates to be spawned at
    int[] possibleY = { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    int[] possibleX = {
         -5, -6, -7, -8, -9, -10, -11, -12, -13, -14, -15, -16, -17,
        5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

    GameObject player;

    float shootDelay = .6f;
    float nextShoot;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            projectilePool.Add(Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity));
            projectilePool[i].SetActive(false);
        }
    }

    void Update()
    {
        if (Time.time > nextShoot)
        {
            nextShoot = Time.time + shootDelay;
            Shoot();
        }
    }

    // Activate projectile at random location, facing at player
    public void Shoot()
    {
        GameObject proj = GetFromPool(projectilePool);
        int x = possibleX[Random.Range(0, possibleX.Length)];
        int y = possibleY[Random.Range(0, possibleY.Length)];
        Vector2 spawnPos = new Vector2(x, y);
        proj.transform.position = spawnPos;
        proj.transform.right = player.transform.position - proj.transform.position;

        proj.SetActive(true);
        
        Projectile projProperties = proj.GetComponent<Projectile>();
        projProperties.Dir = proj.transform.right.normalized;
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
