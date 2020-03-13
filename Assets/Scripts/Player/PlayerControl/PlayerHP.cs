﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHP : MonoBehaviour
{
    [SerializeField]
    int capHP;  // Max HP obtainable, maxHP may be lower but will never exceed cap
    [SerializeField]
    int maxHP;  // Current max HP
    [SerializeField]
    int currentHP;
    [SerializeField]
    Material redMat;
    [SerializeField]
    Material defaultMat;
    [SerializeField]
    Rigidbody2D rb;

    PlayerAuraControl auraControl;
    PlayerMove move;

    float damageRate;
    SpriteRenderer spriteRender;
    bool dead;
    float respawnDelay = 2f;

    // Object pool of damage numbers
    List<GameObject> numPool;
    [SerializeField]
    GameObject numPrefab;
    [SerializeField]
    int poolNum;

    public UnityEvent OnHealthChange;
    public UnityEvent OnDeath;
    public int CurrentHP { get { return currentHP; } }
    public int MaxHP { get { return maxHP; } }
    public float RespawnDelay { get { return respawnDelay; } }

    void Awake()
    {
        move = GetComponent<PlayerMove>();
        auraControl = GetComponent<PlayerAuraControl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize damage numbers pool
        numPool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            numPool.Add(Instantiate(numPrefab, Vector3.zero, Quaternion.identity));
            numPool[i].SetActive(false);
        }

        dead = false;
        currentHP = maxHP;
        damageRate = 0;
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        IsDead();

        //While invulnerable, make transparent
        if (damageRate > Time.time)
        {
            spriteRender.color = new Color(1, 1, 1, .4f);
        }
        else
        {
            spriteRender.color = new Color(1, 1, 1, 1f);
        }
    }

    //If collides with anything that damages player, take damage
    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyDefaults enemyDmg = other.GetComponent<EnemyDefaults>();
        if (enemyDmg != null && Time.time > damageRate && !dead)
        {
            int dmg = enemyDmg.Dmg;
            DisplayDmgNum(dmg); //Show damage number
            currentHP -= dmg; //Calculate HP
            if (currentHP < 0) //Do not go below 0 hp
            {
                currentHP = 0;
            }

            OnHealthChange.Invoke(); // OnHealthChanged event
            move.ResetJump(); //Reset jump on damaged
            damageRate = Time.time + 1f; //Take damage rate
            //Damaged response
            StartCoroutine(Knockback(other.gameObject));
            StartCoroutine(ColorChange());
            StartCoroutine(Invuln());
        }
    }

    //If player stays in anything that damages player, also take damage as if OnTriggerEnter2D
    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    //Show damage number
    void DisplayDmgNum(int dmg)
    {
        if (PlayerPrefs.GetInt("Damage") == 0)
        {
            for (int i = 0; i < numPool.Count; i++)
            {
                if (!numPool[i].activeInHierarchy)
                {
                    numPool[i].GetComponent<TextMeshPro>().text = dmg.ToString();
                    numPool[i].transform.position = transform.position;
                    numPool[i].SetActive(true);
                    return;
                }
            }
        }
    }

    //Execute when player hp reaches 0
    void IsDead()
    {
        if (currentHP <= 0 && !dead)
        {
            dead = true;
            currentHP = 0; //HP displayed cannot be less than 0
            OnDeath.Invoke(); // OnDeath event
            StartCoroutine(Respawn()); //Respawn
        }
    }

    //Respawn player after a delay
    IEnumerator Respawn()
    {
        auraControl.CanAura = false; //Disable aura use

        yield return new WaitForSeconds(respawnDelay);

        //Reset player values and control
        move.Velocity(0, 0);
        move.enabled = true;    //Move should be disabled upon knockback and is not restored if HP drops to 0, so restore it here
        auraControl.CanAura = true;
        currentHP = maxHP;
        dead = false;
        OnHealthChange.Invoke(); // OnHealthChanged event
    }

    //Disable movement, apply knockback, re-enable player movement
    IEnumerator Knockback(GameObject other)
    {
        move.enabled = false;

        // Knockback applied based on player position relative to enemy hit
        float moveX = 10f;
        float moveY = 5f;
        if (other.transform.position.x > transform.position.x)
        {
            moveX = -10f;
        }
        if (other.transform.position.y > transform.position.y && !move.Grounded) // Only knock downwards if in air and below enemy
        {
            moveY = -5f;
        }

        move.Velocity(moveX, moveY);
        yield return new WaitForSeconds(.1f);
        StartCoroutine(Stop(.1f));

        // If player alive, re-enable movement, else keep disabled until respawn
        if (currentHP > 0)
        {
            move.enabled = true;
        }
    }

    //Flash red
    IEnumerator ColorChange()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRender.material = redMat;
            yield return new WaitForSeconds(.05f);
            spriteRender.material = defaultMat;
            if (i < 2)
            {
                yield return new WaitForSeconds(.05f);
            }
        }
    }

    //Change transparency to indicate invulnerability
    IEnumerator Invuln()
    {
        yield return new WaitForSeconds(.15f);
        spriteRender.color = new Color(1, 1, 1, .5f);
        yield return new WaitForSeconds(.1f);
        spriteRender.color = new Color(1, 1, 1, .8f);
        yield return new WaitForSeconds(.1f);
        spriteRender.color = new Color(1, 1, 1, .5f);
        yield return new WaitForSeconds(.1f);
        spriteRender.color = new Color(1, 1, 1, .8f);
        yield return new WaitForSeconds(.1f);
        spriteRender.color = new Color(1, 1, 1, 1f);
    }

    //Hit stop when player is damaged
    IEnumerator Stop(float dur)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(dur);
        Time.timeScale = 1.0f;
    }

    //Reset damage rate
    public void ResetDamageRate()
    {
        damageRate = Time.time + .7f;
    }
}
