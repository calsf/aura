using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStages : MonoBehaviour
{
    EnemyDefaults enemyDefaults;
    Animator anim;

    // Health to reach to activate a new stage, should be in order of highest health to lowest health
    // Example: { 200, 100, 50} = {stage 2, stage 3, stage 4} or index + 2
    [SerializeField]
    int[] healthStages;

    // Start is called before the first frame update
    void Start()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
        anim = GetComponent<Animator>();

        anim.SetInteger("Stage", 1);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < healthStages.Length; i++)
        {
            int currStage = anim.GetInteger("Stage");   // Current boss stage
            int nextStage = (i + 2);  // Starts at stage 1, index + 2 is the stage healthStages value corresponds to

            // Transition stages once lower than certain health, only move up stages, never return back to stages
            if (enemyDefaults.HP < healthStages[i] && nextStage > currStage)
            {
                anim.SetInteger("Stage", nextStage);
            }
        }
    }
}
