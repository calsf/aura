using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeNeck : MonoBehaviour
{
    [SerializeField]
    EnemyDefaults meleeBoss;
    [SerializeField]
    BossStages stages;

    // Update is called once per frame
    void Update()
    {
        // Stretch neck scale to match distance between this object position and the melee boss position
        float x = Mathf.Abs(transform.position.x - meleeBoss.transform.position.x);
        transform.localScale = new Vector2(x, transform.localScale.y);

        // Move y position to same position as melee boss target
        transform.position = new Vector2(transform.position.x, meleeBoss.transform.position.y);

        // Disable after first stage
        if (meleeBoss.HP <= stages.HealthStages[0])
        {
            gameObject.SetActive(false);
        }

    }
}
