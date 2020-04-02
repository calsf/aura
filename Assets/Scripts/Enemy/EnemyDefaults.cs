using System.Collections;
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

    [SerializeField]
    GameObject deathFXPrefab;
    GameObject deathFX;

    // Object pool of damage numbers
    List<GameObject> numPool;
    [SerializeField]
    GameObject numPrefab;
    [SerializeField]
    int poolNum;

    [SerializeField]
    GameObject goldPopupPrefab;
    GameObject goldPopup;
    TextMeshPro goldPopupText;
    LevelManager levelManager;

    int hp;
    SpriteRenderer spriteRender;
    bool collided;
    float damageRate = .2f; // Delay before taking damage again
    float onStayTime; //Delay for OnStay trigger
    float onEnterTime; //Delay for OnEnter trigger

    float moveSpeed;

    public UnityEvent OnDamaged; //OnDamaged event occurs after enemy HP is adjusted

    public Enemy Enemy { get { return enemy; } }
    public int HP { get { return hp; } set { hp = value; } }
    public int Dmg { get { return enemy.dmg; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    // Use this for initialization
    void Awake () {
        hp = enemy.maxHP;
        moveSpeed = enemy.baseMoveSpeed;

        collided = false;
        spriteRender = GetComponent<SpriteRenderer>();
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        // Each enemy has a gold popup for when killed and gold is given to player
        goldPopup = Instantiate(goldPopupPrefab, Vector3.zero, Quaternion.identity);
        goldPopupText = goldPopup.GetComponent<TextMeshPro>();

        // Each enemy has a death fx
        deathFX = Instantiate(deathFXPrefab, Vector3.zero, Quaternion.identity);
        deathFX.SetActive(false);

        // Initialize damage numbers pool
        numPool = new List<GameObject>();
        for (int i = 0; i < poolNum; i++)
        {
            numPool.Add(Instantiate(numPrefab, Vector3.zero, Quaternion.identity));
            numPool[i].SetActive(false);
        }
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
	}


    //*Multiple colliders on an enemy ends up taking multiple damage, onEnterTime avoids this
    //If collides with any player damage, take damage
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "DamageEnemy" && Time.time > onEnterTime)
        {
            onStayTime = Time.time + damageRate; //Delay to avoid OnStay triggering at same time
            onEnterTime = Time.time + damageRate;

            int dmg = other.GetComponentInParent<AuraDefaults>().Dmg;
            DisplayDmgNum(dmg);
            hp -= dmg;
            OnDamaged.Invoke(); //OnDamaged event occurs after enemy HP has been adjusted
            StartCoroutine(ColorChange());
            other.GetComponentInParent<AuraDefaults>().ActivateStop(.03f); // Hitstop
            collided = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "DamageEnemy" && !collided)
        {
            if (Time.time > onStayTime)
            {
                onStayTime = Time.time + damageRate;

                int dmg = other.GetComponentInParent<AuraDefaults>().Dmg;
                DisplayDmgNum(dmg);
                hp -= dmg;
                OnDamaged.Invoke(); //OnDamaged event occurs after enemy HP has been adjusted
                StartCoroutine(ColorChange());
                other.GetComponentInParent<AuraDefaults>().ActivateStop(.03f); // Hitstop
                collided = true;
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

            SoundManager.SoundInstance.PlaySound($"Explo{Random.Range(1, 3)}");

            // Spawn gold pop up on death with a y offset above enemy
            goldPopupText.text = "+" + enemy.gold + " Gold";
            goldPopup.transform.position = transform.position + (Vector3.up * .1f);
            goldPopup.SetActive(true);

            levelManager.Gold += enemy.gold;
            gameObject.SetActive(false);
        }
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
                    numPool[i].GetComponent<TextMeshPro>().text = "-" + dmg.ToString();
                    numPool[i].transform.position = transform.position;
                    numPool[i].SetActive(true);
                    return;
                }
            }
        }
    }

    //Flash on damaged
    IEnumerator ColorChange()
    {
        SoundManager.SoundInstance.PlaySound("Hit");

        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, .4f);
        yield return new WaitForSeconds(.02f);
        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1f);
    }
}
