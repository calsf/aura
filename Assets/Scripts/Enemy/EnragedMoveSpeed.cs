using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Increase enemy move speed multiplier based on missing health

public class EnragedMoveSpeed : MonoBehaviour
{
    // Lower the rate is, the higher the move speed multiplier per missing health
    [SerializeField]
    float rate;

    EnemyDefaults enemyDefaults;

    float moveSpeedMultiplier = 1;

    void Awake()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
    }

    void Update()
    {
        float missingHealth = enemyDefaults.Enemy.maxHP - enemyDefaults.HP;
        moveSpeedMultiplier = 1 + (missingHealth / rate);

        enemyDefaults.MoveSpeedMultiplier = moveSpeedMultiplier;
    }
}
