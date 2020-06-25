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

    bool move;

    void Awake()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
        targetReachedEffect = Instantiate(targetReachedEffectPrefab, transform.position, Quaternion.identity);
        targetReachedEffect.SetActive(false);
    }

    void OnEnable()
    {
        move = false;

        // Heal bugs are child of boss, make sure local scale is positive so bugs are not flipped wrong way
        target.transform.localScale = target.transform.localScale.x < 0 ?
            new Vector3(-target.transform.localScale.x, target.transform.localScale.y) :
            target.transform.localScale;

        // Face bug in direction of assigned target
        if ((transform.localScale.x > 0 && transform.localPosition.x < target.gameObject.transform.position.x)
                || (transform.localScale.x < 0 && transform.localPosition.x > target.gameObject.transform.position.x))
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!move)
        {
            return;
        }

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

    public void StartMove()
    {
        move = true;
    }
}
