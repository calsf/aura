using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move to target, once reached, disable object, enable another object to show target reached and heal the target

public class HealBugMove : MonoBehaviour
{
    [SerializeField]
    EnemyDefaults target;

    [SerializeField]
    GameObject targetReachedEffectPrefab;
    GameObject targetReachedEffect;

    EnemyDefaults enemyDefaults;

    void Awake()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
        targetReachedEffect = Instantiate(targetReachedEffectPrefab, transform.position, Quaternion.identity);
        targetReachedEffect.SetActive(false);
    }

    void OnEnable()
    {
        // Face bug in direction of assigned target
        if ((transform.localScale.x > 0 && transform.position.x < target.transform.position.x)
            || (transform.localScale.x < 0 && transform.position.x > target.transform.position.x))
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, enemyDefaults.MoveSpeed * Time.deltaTime);

        // If reached target, disable this object and spawn the gameobject to show that target was reached
        // Heal target for same amount as enemy health
        if (Vector3.Distance(transform.position, target.transform.position) <= 0.1f)
        {
            // Heal target
            target.HP += enemyDefaults.Enemy.maxHP;
            target.DisplayHealNum(enemyDefaults.Enemy.maxHP);

            // Deactive this object
            targetReachedEffect.transform.position = transform.position;
            targetReachedEffect.SetActive(true);
            gameObject.SetActive(false);
        }

    }
}
