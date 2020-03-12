using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* EnemyHPManager should be placed on enemy object with healthbar object as child of enemy
 * Healthbar object with health bar fill and background objects as children
 */

public class EnemyHPManager : MonoBehaviour
{
    EnemyDefaults enemy;
    GameObject barObject;
    float hideTime = 3.5f;
    float lastHit = 0;

    void Awake()
    {
        barObject = transform.GetChild(0).gameObject;
        enemy = GetComponent<EnemyDefaults>();
    }

    void OnEnable()
    {
        enemy.OnDamaged.AddListener(ShowHealth);
    }

    void OnDisable()
    {
        enemy.OnDamaged.RemoveListener(ShowHealth);
    }

    // Update is called once per frame
    void Update()
    {
        // Flip health bar along with enemy flip (this has opposite effect, will actually prevent health bar from flipping)
        if (enemy.gameObject.transform.localScale.x < 0 && transform.localScale.x > 0 || enemy.gameObject.transform.localScale.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

        // After some time from not being hit, hide enemy health bar
        if (Time.time > lastHit)
        {
            barObject.SetActive(false);
        }
    }

    // Show enemy health bar if not active, if health bar already active, reset the lastHit timer
    void ShowHealth()
    {
        if (!barObject.activeInHierarchy)
        {
            lastHit = Time.time + hideTime;
            barObject.SetActive(true);
        }
        else
        {
            lastHit = Time.time + hideTime;
        }
    }
}
