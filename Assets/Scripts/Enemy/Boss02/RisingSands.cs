using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingSands : MonoBehaviour
{
    [SerializeField]
    Transform mainObject;

    [SerializeField]
    GameObject sandsParent;
    GameObject[] sands;
    bool isActive;

    float defaultDelay = .05f;
    float delay;
    float nextActive;

    int startPoint;
    int curr;

    void Awake()
    {
        sands = new GameObject[sandsParent.transform.childCount];
        for (int i = 0; i < sands.Length; i++)
        {
            sands[i] = sandsParent.transform.GetChild(i).gameObject;
        }
    }

    void OnEnable()
    {
        // Depending on enemy facing direction, start from end of beginning of sands array
        startPoint = mainObject.localScale.x > 0 ? sands.Length - 1 : 0;
        curr = startPoint;
        isActive = true;
        nextActive = 0;

        delay = defaultDelay;
    }

    void Update()
    {
        // Slow down sands activation if enemy gets slowed
        delay = defaultDelay 
            / (mainObject.gameObject.GetComponent<EnemyDefaults>().MoveSpeed / mainObject.gameObject.GetComponent<EnemyDefaults>().Enemy.baseMoveSpeed);

        // Activate each sands object
        if (isActive && Time.time > nextActive)
        {
            if (startPoint == 0)
            { 
                if (curr < sands.Length-1 && !sands[curr].activeInHierarchy)
                {
                    nextActive = Time.time + delay;
                    sands[curr].SetActive(true);
                    curr++;
                }
            }
            else
            {
                if (curr > 0 && !sands[curr].activeInHierarchy)
                {
                    nextActive = Time.time + delay;
                    sands[curr].SetActive(true);
                    curr--;
                }
            }
        }
         
    }

}
