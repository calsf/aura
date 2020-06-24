using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manager for heal bugs

public class HealBugsManager : MonoBehaviour
{
    [SerializeField]
    GameObject enemyToHeal;
    [SerializeField]
    Animator enemyToHealAnim;
    [SerializeField]
    string finishedHealingAnim;

    EnemyDefaults[] bugs;
    bool isFinished;    // Has finished healing with all heal bugs?

    // Possible x and y coordinates to be spawned at
    int[] possibleY = { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7 };
    int[] possibleX = {
        -6, -7, -8, -9, -10, -11, -12, -13, -14, -15, -16, -17,
        6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

    bool started;

    void Start()
    {
        bugs = new EnemyDefaults[transform.childCount];
        for (int i = 0; i < bugs.Length; i++)
        {
            bugs[i] = transform.GetChild(i).GetComponentInChildren<EnemyDefaults>();
        }

        isFinished = true;
        started = true;
    }

    void Update()
    {
        // If not finished healing, check if all bugs are gone
        if (!isFinished)
        {
            foreach (EnemyDefaults bug in bugs)
            {
                // If any bug is still active, return
                if (bug.gameObject.activeInHierarchy)
                {
                    return;
                }
            }

            // If reached here, no bug is active and finished healing
            isFinished = true;
            enemyToHealAnim.Play(finishedHealingAnim);
        }
    }

    void OnEnable()
    {
        if (!started)
        {
            return;
        }

        List<Vector2> spawned = new List<Vector2>();

        // Spawn every bug in a random position, do not spawn into same position as another bug
        foreach (EnemyDefaults bug in bugs)
        {
            Vector2 spawnPos;
            do
            {
                int x = possibleX[Random.Range(0, possibleX.Length)];
                int y = possibleY[Random.Range(0, possibleY.Length)];
                spawnPos = new Vector2(x, y);
            }
            while (spawned.Contains(spawnPos));

            bug.gameObject.transform.position = spawnPos;
            spawned.Add(spawnPos);

            bug.gameObject.SetActive(true);
        }

        // Set to false to indicate still healing
        isFinished = false;
    }

    void OnDisable()
    {
        // Reset enemy hp and disable
        foreach (EnemyDefaults bug in bugs)
        {
            bug.HP = bug.Enemy.maxHP;
            bug.gameObject.SetActive(false);
        }
    }

}
