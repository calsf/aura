using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Meteoric aura spawns meteor damage when player lands on ground, aura itself is never active
// Meteoric damage objects should have AuraDefaults attached but disabled so it can deal damage but not follow player position

public class MeteoricAura : MonoBehaviour
{
    [SerializeField]
    Transform meteorSpawn;
    PlayerMoveInput playerMove;
    PlayerController playerController;
    Animator anim;

    [SerializeField]
    GameObject meteorPrefab;
    List<GameObject> meteorPool;
    [SerializeField]
    int poolNum;

    void Awake()
    {
        playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveInput>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        // Init meteor pool
        meteorPool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            meteorPool.Add(Instantiate(meteorPrefab, Vector3.down * 80, Quaternion.identity));
            
        }
    }

    void Start()
    {
        // Disable aura behaviour but make sure dmg is set
        foreach (GameObject meteor in meteorPool)
        {
            AuraDefaults aura = meteor.GetComponent<AuraDefaults>();
            aura.SetDmg();
            aura.enabled = false;
            meteor.SetActive(false);
        }
    }

    void Update()
    {
        // If player is falling, play falling animation
        if (playerMove.Velocity.y < 0 && !playerController.Collisions.below)
        {
            anim.SetBool("PlayerFalling", true);
        }
        else
        {
            anim.SetBool("PlayerFalling", false);

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
        GameObject newObj = Instantiate(meteorPrefab, Vector3.zero, Quaternion.identity);
        meteorPool.Add(newObj);
        return newObj;
    }

    // Spawn meteor on landing which will do damage
    public void Landing()
    {
        GameObject flame = GetFromPool(meteorPool);
        flame.transform.position = meteorSpawn.position;
        flame.SetActive(true);
    }
}
