using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseOnHit : MonoBehaviour
{
    GameObject player;
    EnemyDefaults enemyDefaults;
    Animator anim;
    bool isStartingAggro;
    bool isAggro;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyDefaults = GetComponent<EnemyDefaults>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Once lost HP, aggro onto player and chase them
        if (!isStartingAggro && enemyDefaults.HP < enemyDefaults.Enemy.maxHP)
        {
            isStartingAggro = true;
            StartAggro();
        }
    }

    void FixedUpdate()
    {
        if (isStartingAggro || isAggro)
        {
            //Swap facing x direction if necessary
            if ((transform.localScale.x > 0 && transform.position.x < player.transform.position.x) || (transform.localScale.x < 0 && transform.position.x > player.transform.position.x))
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }

        if (!isAggro)
        {
            return;
        }

        // Chase player if aggro'd
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, enemyDefaults.MoveSpeed * Time.deltaTime);
    }

    // Play aggro animation
    public void StartAggro()
    {
        anim.Play("StartAggro"); // ANIMATION STATE MUST BE SAME NAME - ANIMATION CLIPS CAN BE DIFFERENT NAME
    }

    // Turn isAggro on in animation
    public void ToggleAggro()
    {
        isAggro = true;
    }
}
