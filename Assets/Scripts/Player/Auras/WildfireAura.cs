using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns WildfireFlame whenever an enemy is hit

public class WildfireAura : MonoBehaviour
{
    [SerializeField]
    GameObject flamePrefab;
    List<GameObject> flamePool;
    [SerializeField]
    int poolNum;

    [SerializeField]
    LayerMask flameMask;

    void Awake()
    {
        flamePool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            flamePool.Add(Instantiate(flamePrefab, Vector3.zero, Quaternion.identity));
            flamePool[i].SetActive(false);
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
        GameObject newObj = Instantiate(flamePrefab, Vector3.zero, Quaternion.identity);
        flamePool.Add(newObj);
        return newObj;
    }

    // Spawn wildfire flame at other position
    public void SpawnFlame(Transform other)
    {
        // Check if there is already a wildfire flame on top of other position, if there is, return and do not spawn flame
        RaycastHit2D hit = Physics2D.Raycast(other.gameObject.transform.position, Vector2.zero, 0, flameMask);
        if (hit.collider != null && hit.collider.gameObject.activeInHierarchy)
        {
            return;
        }

        GameObject flame = GetFromPool(flamePool);
        flame.transform.position = other.position;
        flame.SetActive(true);
    }

}
