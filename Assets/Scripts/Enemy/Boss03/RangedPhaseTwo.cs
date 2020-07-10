using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedPhaseTwo : MonoBehaviour
{
    [SerializeField]
    DamagePlayerDefaults[] orbs;
    Animator[] orbAnim;
    Vector2[] orbPos;

    // Object pool
    List<GameObject> deathFXPool;
    [SerializeField]
    GameObject deathFXPrefab;
    [SerializeField]
    int poolNum;


    // Possible x and y coordinates to move to
    int[] possibleY = { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12 };
    int[] possibleX = {
         -15, -14, -13, -12, -11, -10, -9, -8, -7, -6, -5, -4, -3, -2, -1,
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

    [SerializeField]
    Transform spawnPos1;
    [SerializeField]
    Transform spawnPos2;

    bool isAttacking;

    [SerializeField]
    float delay;
    float nextAttack;

    Animator anim;

    void Awake()
    {
        orbPos = new Vector2[orbs.Length];
        anim = GetComponent<Animator>();

        orbAnim = new Animator[orbs.Length];
        for (int i = 0; i < orbs.Length; i++)
        {
            orbAnim[i] = orbs[i].gameObject.GetComponent<Animator>();
        }

        deathFXPool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            deathFXPool.Add(Instantiate(deathFXPrefab, Vector3.down * 50, Quaternion.identity));
            deathFXPool[i].SetActive(false);
        }
    }

    void OnEnable()
    {
        nextAttack = Time.time + (delay / 2);
    }

    void OnDisable()
    {
        foreach(DamagePlayerDefaults orb in orbs)
        {
            GameObject deathFX = GetFromPool(deathFXPool);
            deathFX.transform.position = orb.gameObject.transform.position;
            orb.gameObject.SetActive(false);
            deathFX.SetActive(true);
        }
    }

    void Update()
    {
        if (!isAttacking && Time.time > nextAttack)
        {
            // Start attack
            anim.Play("RangedPhaseTwoAttack");
        }
        else if (isAttacking)
        {
            bool inPosition = true;

            // Move each orb, rotating at random
            for (int i = 0; i < orbs.Length; i++)
            {
                orbs[i].transform.position = Vector3.MoveTowards(orbs[i].transform.position, orbPos[i], orbs[i].Speed * Time.deltaTime);
                orbs[i].transform.Rotate(0, 0, Random.Range(6, 13));
            }

            // Check if orbs are in target positions, if any orb is not, then is not in position
            for (int i = 0; i < orbs.Length; i++)
            {
                if ((Vector2)orbs[i].transform.position != orbPos[i])
                {
                    inPosition = false;
                }
            }

            // Activate lasers once all orbs in target destination
            if (inPosition)
            {
                foreach (Animator orb in orbAnim)
                {
                    orb.Play("LaserOrbActivate");
                }

                SoundManager.SoundInstance.PlaySound("RangedLaser");

                // Reset attack
                isAttacking = false;
                nextAttack = Time.time + delay;
            }

        }
    }


    // Launch orbs via animation
    public void LaunchOrbs()
    {
        SoundManager.SoundInstance.PlaySound("RangedShootOrbs");

        List<Vector2> currPositions = new List<Vector2>();
        Transform lastSpawn = spawnPos1;

        // Move each orb to a random position, do not move into same position as another bug
        for (int i = 0; i < orbs.Length; i++)
        {
            Vector2 moveTo;
            do
            {
                int x = possibleX[Random.Range(0, possibleX.Length)];
                int y = possibleY[Random.Range(0, possibleY.Length)];
                moveTo = new Vector2(x, y);
            }
            while (currPositions.Contains(moveTo));

            orbs[i].gameObject.transform.position = lastSpawn.position;
            lastSpawn = lastSpawn == spawnPos1 ? spawnPos2 : spawnPos1;

            orbPos[i] = moveTo;
            currPositions.Add(moveTo);

            orbs[i].gameObject.SetActive(true);
        }

        isAttacking = true;
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

        return null;
    }
}
