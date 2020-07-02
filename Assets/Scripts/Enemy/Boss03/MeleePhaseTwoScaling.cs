using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Scale melee phase two move speed, rotation, and move delay based on enemy missing health from the melee phase two health stage

public class MeleePhaseTwoScaling : MonoBehaviour
{
    // Lower the rate is, the higher the move speed multiplier per missing health
    [SerializeField]
    float moveRate;

    // Lower the rate is, the lower/faster the move delay
    [SerializeField]
    float delayRate;

    [SerializeField]
    float baseRotation;

    EnemyDefaults enemyDefaults;
    BossStages bossStage;

    float moveSpeedMultiplier = 1;

    GameObject barObject;
    MeleePhaseTwo phaseTwo;
    float originalMoveDelay;

    void Awake()
    {
        bossStage = GetComponent<BossStages>();
        enemyDefaults = GetComponent<EnemyDefaults>();

        phaseTwo = GetComponent<MeleePhaseTwo>();
        barObject = transform.GetChild(0).gameObject;   // barObject MUST BE FIRST CHILD OF OBJECT THIS SCRIPT IS ATTACHED TO

        originalMoveDelay = phaseTwo.MoveDelay;
    }

    void Update()
    {
        if (enemyDefaults.HP > bossStage.HealthStages[0])
        {
            return;
        }

        float missingHealth = bossStage.HealthStages[0] - enemyDefaults.HP;
        moveSpeedMultiplier = 1 + (missingHealth / moveRate);

        // Apply new move speed multiplier
        enemyDefaults.MoveSpeedMultiplier = moveSpeedMultiplier;

        // Lower move delay
        phaseTwo.MoveDelay = originalMoveDelay - (missingHealth / delayRate);

        // Unparent the enemy health bar and rotate enemy itself, then reattach the health bar to maintain fixed health position
        barObject.transform.parent = null;
        transform.Rotate(0, 0, baseRotation * moveSpeedMultiplier);
        barObject.transform.parent = transform;
        
    }
}
