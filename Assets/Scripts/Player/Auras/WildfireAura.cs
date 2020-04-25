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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            // Check if there is already a wildfire flame on top of enemy position
            RaycastHit2D hit = Physics2D.Raycast(other.gameObject.transform.position, Vector2.zero, 0, flameMask); //Raycast to detect any objects at mouse position
            if (hit.collider != null)
            {
                return;
            }

            // Spawn a wildfire flame every time aura hits an enemy
            if (Time.time > other.GetComponent<EnemyDefaults>().OnEnterTime)
            {
                GameObject flame = GetFromPool(flamePool);
                flame.transform.position = other.gameObject.transform.position;
                flame.SetActive(true);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

}
