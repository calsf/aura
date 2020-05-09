﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

/* Defaults of enemy
 * Values for an enemy obtained from enemy scriptable object
 * */

public class EnemyDefaults : MonoBehaviour {
    [SerializeField]
    Enemy enemy;
    Material defaultMat;

    GameObject deathFX;

    // Object pool of damage numbers
    List<GameObject> numPool;
    [SerializeField]
    GameObject numPrefab;
    int poolNum = 30;

    // Object pool of heal numbers
    List<GameObject> healPool;
    [SerializeField]
    GameObject healNumPrefab;
    int healPoolNum = 10;

    [SerializeField]
    GameObject goldPopupPrefab;
    GameObject goldPopup;
    TextMeshPro goldPopupText;
    LevelManager levelManager;

    int hp;
    SpriteRenderer spriteRender;
    bool collided;
    float damageRate = .1f; // Delay before taking damage again
    float onStayTime; //Delay for OnStay trigger
    float onEnterTime; //Delay for OnEnter trigger

    float restoreHPTime = 0;
    int restoreHPAmount;

    // Delay for taking extra damage (e.g: wildfire flames should deal damage without affecting damage delay from auras)
    float onEnterTimeExtra;
    float onStayTimeExtra;

    float moveSpeed;
    float restoreMoveSpeedTime = 0; // The time at which move speed should be restored
    float restoreRate;

    AudioSource[] audioSources;

    public UnityEvent OnDamaged; //OnDamaged event occurs after enemy HP is adjusted

    public Enemy Enemy { get { return enemy; } }
    public int HP { get { return hp; } set { hp = value; } }
    public int Dmg { get { return enemy.dmg; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float OnEnterTime { get { return onEnterTime; } }
    public float RestoreMoveSpeedTime { get { return restoreMoveSpeedTime; } set { restoreMoveSpeedTime = value; } }
    public float RestoreHPTime { get { return restoreHPTime; } set { restoreHPTime = value; } }
    public int RestoreHPAmount { get { return restoreHPAmount; } set { restoreHPAmount = value; } }
    public SpriteRenderer SpriteRender { get { return spriteRender; } }

    // Use this for initialization
    void Awake () {
        hp = enemy.maxHP;
        moveSpeed = enemy.baseMoveSpeed;
        restoreRate = enemy.baseMoveSpeed / 50f; // Restore rate relative to base speed
        

        collided = false;
        spriteRender = GetComponent<SpriteRenderer>();
        defaultMat = spriteRender.material;     // Get enemy's default starting material
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        // Each enemy has a gold popup for when killed and gold is given to player
        goldPopup = Instantiate(goldPopupPrefab, Vector3.zero, Quaternion.identity);
        goldPopupText = goldPopup.GetComponent<TextMeshPro>();

        // Each enemy has a death fx
        deathFX = Instantiate(enemy.deathFX, Vector3.zero, Quaternion.identity);
        deathFX.SetActive(false);

        // Initialize damage numbers pool
        numPool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            numPool.Add(Instantiate(numPrefab, Vector3.zero, Quaternion.identity));
            numPool[i].SetActive(false);
        }

        // Initialize heal numbers pool
        healPool = new List<GameObject>();
        for (int i = 0; i < healPoolNum; i++)
        {
            healPool.Add(Instantiate(healNumPrefab, Vector3.zero, Quaternion.identity));
            healPool[i].SetActive(false);
        }


        // Create audio sources attached to enemy with their respective damaged and death sounds
        // Attaching audio sources to each enemy will allow for spatial sounds
        // Volume is set before audio clips are actually played to account for sound volume setting
        audioSources = new AudioSource[4];

        // First 3 audio clips are hit sounds
        for (int i = 0; i < audioSources.Length - 1; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].clip = enemy.hitSounds[i];
            audioSources[i].spatialBlend = 1;
            audioSources[i].pitch = 1f;

            audioSources[i].rolloffMode = AudioRolloffMode.Custom;
            audioSources[i].maxDistance = 30;
        }

        // Last audio clip will be death sound
        // Death sound is added to the deathFX object so that the sound can be played after enemy is dead and disabled
        audioSources[audioSources.Length-1] = deathFX.AddComponent<AudioSource>();
        audioSources[audioSources.Length-1].clip = enemy.deathSound;
        audioSources[audioSources.Length-1].spatialBlend = 1;
        audioSources[audioSources.Length-1].rolloffMode = AudioRolloffMode.Custom;
        audioSources[audioSources.Length-1].maxDistance = 30;

    }

    void Start()
    {
        // Deactive gold popup after
        goldPopup.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        collided = false;
        IsDead();

        // Restore hp if was affected from demi aura
        if (Time.time > restoreHPTime && spriteRender.material != defaultMat)
        {
            hp += restoreHPAmount;
            if (restoreHPAmount > 0)
            {
                DisplayHealNum(restoreHPAmount);
            }
            spriteRender.material = defaultMat;
            restoreHPAmount = 0;
        }
	}

    void FixedUpdate()
    {
        // Gradually restore speed for a decaying slow effect from slows
        if (moveSpeed < enemy.baseMoveSpeed && Time.time > restoreMoveSpeedTime)
        {
            moveSpeed += restoreRate;
            if (moveSpeed > enemy.baseMoveSpeed)
            {
                moveSpeed = enemy.baseMoveSpeed;
            }
        }
    }


    //*Multiple colliders on an enemy ends up taking multiple damage, onEnterTime avoids this
    //If collides with any player damage, take damage
    void OnTriggerEnter2D(Collider2D other)
    {
        // Separate damage rate for non-aura damage like wildfire flames
        if (other.tag == "ExtraDamage" && Time.time > onEnterTimeExtra)
        {
            onStayTimeExtra = Time.time + damageRate;
            onEnterTimeExtra = Time.time + damageRate;
            int dmg = other.GetComponent<WildfireFlame>().Dmg;
            DisplayDmgNum(dmg);
            hp -= dmg;
            OnDamaged.Invoke(); //OnDamaged event occurs after enemy HP has been adjusted
            StartCoroutine(ColorChange());
        }

        // Damage rate for direct aura damage
        if (other.tag == "DamageEnemy" && Time.time > onEnterTime)
        {
            // Does not register as damaged if dmg is 0
            int dmg = other.GetComponentInParent<AuraDefaults>().Dmg;
            if (dmg > 0)
            {
                onStayTime = Time.time + damageRate; //Delay to avoid OnStay triggering at same time
                onEnterTime = Time.time + damageRate;

                // Spawn flame if was hit by wildfire aura
                WildfireAura wfAura = other.GetComponent<WildfireAura>();
                if (wfAura != null)
                {
                    // Spawn flame at collision point
                    wfAura.SpawnFlame(other.ClosestPoint(transform.position));
                }

                // Deal damage to enemy

                DisplayDmgNum(dmg);
                hp -= dmg;
                OnDamaged.Invoke(); //OnDamaged event occurs after enemy HP has been adjusted
                StartCoroutine(ColorChange());
                collided = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Separate damage rate for non-aura damage like wildfire flames
        if (other.tag == "ExtraDamage")
        {
            if (Time.time > onStayTimeExtra)
            {
                onStayTimeExtra = Time.time + damageRate;

                int dmg = other.GetComponent<WildfireFlame>().Dmg;
                DisplayDmgNum(dmg);
                hp -= dmg;
                OnDamaged.Invoke(); //OnDamaged event occurs after enemy HP has been adjusted
                StartCoroutine(ColorChange());
            }
        }

        // Damage rate for direct aura damage
        if (other.tag == "DamageEnemy" && !collided)
        {
            if (Time.time > onStayTime)
            {
                // Does not register as damaged if dmg is 0
                int dmg = other.GetComponentInParent<AuraDefaults>().Dmg;
                if (dmg > 0)
                {
                    onStayTime = Time.time + damageRate;

                    // Spawn flame if was hit by wildfire aura
                    WildfireAura wfAura = other.GetComponent<WildfireAura>();
                    if (wfAura != null)
                    {
                        // Spawn flame at collision point
                        wfAura.SpawnFlame(other.ClosestPoint(transform.position));
                    }

                    // Deal damage to enemy
                    DisplayDmgNum(dmg);
                    hp -= dmg;
                    OnDamaged.Invoke(); //OnDamaged event occurs after enemy HP has been adjusted
                    StartCoroutine(ColorChange());
                    collided = true;
                }
            }
        }
    }

    //Destroys object if hp is 0 or less
    void IsDead()
    {
        if(hp <= 0)
        {
            // Activate deathFX object
            deathFX.transform.position = transform.position;
            deathFX.SetActive(true);

            // Set audio volume according to volume setting then play enemy's death sound clip
            audioSources[audioSources.Length-1].volume = enemy.deathVolume * PlayerPrefs.GetInt("SoundVolume", 10) / 10f;
            audioSources[audioSources.Length-1].Play();

            // Spawn gold pop up on death with a y offset above enemy
            if (enemy.gold > 0)
            {
                goldPopupText.text = "+" + enemy.gold + " Gold";
                goldPopup.transform.position = transform.position + (Vector3.up * .1f);
                goldPopup.SetActive(true);
            }

            levelManager.Gold += enemy.gold;
            gameObject.SetActive(false);
        }
    }

    //Show damage number
    public void DisplayDmgNum(int dmg)
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

    //Show heal number
    public void DisplayHealNum(int heal)
    {
        for (int i = 0; i < healPool.Count; i++)
        {
            if (!healPool[i].activeInHierarchy)
            {
                healPool[i].GetComponent<TextMeshPro>().text = "+" + heal.ToString();
                healPool[i].transform.position = transform.position;
                healPool[i].SetActive(true);
                return;
            }
        }
    }

    //Flash on damaged
    public IEnumerator ColorChange()
    {
        // Choose random hit sound, set volume according to the sound volume setting then play a random enemy hit sound
        int hitSound = Random.Range(0, 3);
        audioSources[hitSound].volume = .4f * PlayerPrefs.GetInt("SoundVolume", 10) / 10f;
        audioSources[hitSound].Play();

        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, .4f);
        yield return new WaitForSeconds(.08f);
        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1f);
    }

    // If case other script such as demi aura wants to activate color change, makes sure that the coroutine is not suddenly stopped by calling it from enemy
    public void StartColorChange()
    {
        StartCoroutine(ColorChange());
    }
}
