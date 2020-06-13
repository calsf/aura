using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TriggerOnHit : MonoBehaviour
{
    [SerializeField]
    float growRate;
    EnemyHPManager enemyHP;

    void Awake()
    {
        enemyHP = GetComponent<EnemyHPManager>();
    }

    // OnDamaged event
    public void GrowOnHit()
    {
        // Increase size by grow rate when hit
        if (transform.localScale.x < 1)
        {
            // Keep health bar static
            enemyHP.BarObject.transform.parent = null;
            transform.localScale = new Vector2(transform.localScale.x + growRate, transform.localScale.y + growRate);
            enemyHP.BarObject.transform.parent = transform;
        }
    }
}
