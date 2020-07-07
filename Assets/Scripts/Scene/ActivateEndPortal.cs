using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// After an enemy is defeated, activates end portal after a delay

public class ActivateEndPortal : MonoBehaviour
{
    [SerializeField]
    GameObject portal;

    [SerializeField]
    float delay;
    float activateTime;

    [SerializeField]
    EnemyDefaults[] enemyDefaults;

    bool hasActivated = false;

    // Update is called once per frame
    void Update()
    {
        // Once all enemy defeated, set time to activate end portal
        if (!hasActivated)
        {
            foreach (EnemyDefaults enemy in enemyDefaults)
            {
                if (enemy.HP > 0)
                {
                    return;
                }
            }

            hasActivated = true;
            activateTime = Time.time + delay;
        }

        // Activate end portal
        if (hasActivated && !portal.activeInHierarchy && Time.time > activateTime)
        {
            portal.SetActive(true);
        }
    }
}
