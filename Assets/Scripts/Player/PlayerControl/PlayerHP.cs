using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHP : MonoBehaviour
{
    int maxHP;  // Current max HP
    [SerializeField]
    int currentHP;
    [SerializeField]
    Material redMat;
    [SerializeField]
    Material defaultMat;
    [SerializeField]
    Material invulnMat;

    PlayerAuraControl auraControl;
    PlayerMoveInput move;
    PlayerController controller;

    float damagedTime;
    float damagedDelay = 1.5f;
    SpriteRenderer spriteRender;
    bool dead;
    float respawnDelay = 4f;
    bool hasFlashed;    // Has finished flashing red

    // Object pool of damage numbers
    List<GameObject> numPool;
    [SerializeField]
    GameObject numPrefab;
    [SerializeField]
    int poolNum;

    // Determine if should leave on death, if leave on death is on, will need menuNav to disable menu as it loads zone select
    [SerializeField]
    bool leaveOnDeath;
    [SerializeField]
    MenuNav menuNav;

    public UnityEvent OnHealthChange;
    public UnityEvent OnDeath;
    public UnityEvent OnRespawning;
    public UnityEvent OnSpawn;
    public int CurrentHP { get { return currentHP; } set { currentHP = value; } }
    public int MaxHP { get { return maxHP; } }
    public float RespawnDelay { get { return respawnDelay; } }
    public float DamagedTime { get { return damagedTime; } set { damagedTime = value; } }
    public bool LeaveOnDeath { get { return leaveOnDeath; } }

    void Awake()
    {
        move = GetComponent<PlayerMoveInput>();
        controller = GetComponent<PlayerController>();
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

        maxHP = SaveLoadManager.LoadHealth();    // Get saved max hp value
        dead = false;
        currentHP = maxHP;
        damagedTime = 0;
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        IsDead();

        //While invulnerable, change shader material to show player is invulnerable
        if (currentHP > 0 && hasFlashed && damagedTime > Time.time)
        {
            spriteRender.material = invulnMat;
        }
        else if (Time.time > damagedTime)
        {
            // Return to default material after invulnerability runs out
            hasFlashed = false;
            spriteRender.material = defaultMat;
        }
    }

    //If collides with anything that damages player, take damage
    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyDefaults enemyDmg = other.GetComponent<EnemyDefaults>();
        DamagePlayerDefaults dmgPlayer = other.GetComponent<DamagePlayerDefaults>();

        bool isEnemy = enemyDmg != null ? true : false;

        if ((enemyDmg != null || dmgPlayer != null) && Time.time > damagedTime && !dead)
        {
            int dmg = isEnemy ? enemyDmg.Dmg : dmgPlayer.Dmg;

            // If dmg is 0 or less, do nothing and return
            if (dmg <= 0)
            {
                return;
            }

            DisplayDmgNum(dmg); //Show damage number
            currentHP -= dmg; //Calculate HP
            if (currentHP < 0) //Do not go below 0 hp
            {
                currentHP = 0;
            }

            OnHealthChange.Invoke(); // OnHealthChanged event
            damagedTime = Time.time + damagedDelay; //Take damage rate
            //Damaged response
            PlaySound();
            StartCoroutine(Knockback(other.gameObject));
            StartCoroutine(FlashRed());
        }
    }

    //If player stays in anything that damages player, also take damage as if OnTriggerEnter2D
    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    //Show damage number
    public void DisplayDmgNum(int dmg)
    {
        if (PlayerPrefs.GetInt("Damage") == 0)
        {
            for (int i = 0; i < numPool.Count; i++)
            {
                if (!numPool[i].activeInHierarchy)
                {
                    numPool[i].GetComponent<TextMeshPro>().text = "-" + dmg.ToString();
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

    // Play hit sound when player is hit, if player dies, play death sound instead
    void PlaySound()
    {
        if (currentHP > 0)
        {
            SoundManager.SoundInstance.PlaySound("PlayerHit");
        }
        else
        {
            SoundManager.SoundInstance.PlaySound("PlayerDeath");
        }
    }

    //Respawn player after a delay
    IEnumerator Respawn()
    {
        auraControl.CanAura = false; //Disable aura use

        // Total wait time must still be equal to respawnDelay, LevelManager waits same amount of time OnDeath
        yield return new WaitForSeconds(respawnDelay - 2f);
        OnRespawning.Invoke();  // Right before respawning ( activate animation )
        SoundManager.SoundInstance.PlaySound("BaseAuraOn");
        yield return new WaitForSeconds(2f);    // Time of animations before respawning

        // Return to zone select on death or respawn in same level
        if (leaveOnDeath)
        {
            menuNav.IsLeaving = true;
            LoadLevel.LoadInstance.LoadScene(1);
        }
        else
        {
            //Reset player values and control
            OnSpawn.Invoke();   // After respawned
            move.Velocity = Vector2.zero;
            move.CanInput = true;    //Move canInput should be disabled upon knockback and is not restored if HP drops to 0, so restore it here
            auraControl.CanAura = true;
            currentHP = maxHP;
            dead = false;
            OnHealthChange.Invoke(); // OnHealthChanged event
        }
    }

    // Knockback on damaged - disable movement, apply knockback, re-enable player movement
    IEnumerator Knockback(GameObject other)
    {
        move.CanInput = false;

        // Knockback applied based on player position relative to enemy hit
        float moveX = 10f;
        float moveY = 5f;
        if (other.transform.position.x > transform.position.x)
        {
            moveX = -10f;
        }
        if (other.transform.position.y > transform.position.y && !controller.Collisions.below) // Only knock downwards if in air and below enemy
        {
            moveY = -5f;
        }

        move.Velocity = new Vector2(moveX, moveY);
        yield return new WaitForSeconds(.1f);
        StartCoroutine(Stop(.1f)); // Player movement should be re-enabled after hit stop
    }

    //Flash red on damaged
    IEnumerator FlashRed()
    {
        int times = 3;
        for (int i = 0; i < times; i++)
        {
            spriteRender.material = redMat;
            yield return new WaitForSeconds(.05f);
            spriteRender.material = defaultMat;
            if (i < times - 1)
            {
                yield return new WaitForSeconds(.05f);
            }
        }
        hasFlashed = true;
    }

    //Hit stop when player is damaged
    IEnumerator Stop(float dur)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(dur);
        Time.timeScale = 1.0f;

        // If player alive, re-enable movement, else keep disabled until respawn
        if (currentHP > 0)
        {
            move.Velocity = new Vector2(0, 0);
            move.CanInput = true;
        }
        else
        {
            move.Velocity = new Vector2(0, move.Velocity.y);
        }
    }

    //Reset damage rate
    public void ResetDamageRate()
    {
        damagedTime = Time.time + .7f;
    }
}
