using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ActivateEndPortal : MonoBehaviour
{
    [SerializeField]
    GameObject portal;

    [SerializeField]
    float delay;
    float activateTime;

    [SerializeField]
    EnemyDefaults enemyDefaults;

    bool hasActivated = false;

    // Update is called once per frame
    void Update()
    {
        if (enemyDefaults.HP <= 0 && !hasActivated)
        {
            hasActivated = true;
            activateTime = Time.time + delay;
        }

        if (hasActivated && !portal.activeInHierarchy && Time.time > activateTime)
        {
            portal.SetActive(true);
        }
    }
}
