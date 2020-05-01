using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Default properties of an aura, will be on every aura

public class AuraDefaults : MonoBehaviour
{
    [SerializeField]
    Aura aura;
    GameObject player;
    int dmg;
    bool waiting;

    public int Dmg { get { return dmg; } set { dmg = value; } }
    public GameObject Player { get { return player; } }
    public Aura Aura { get { return aura; } }

    void OnEnable()
    {
        transform.position = player.transform.position;
    }

    void OnDisable()
    {
        Time.timeScale = 1;
    }

    // Init
    void Awake()
    {
        dmg = aura.baseDmg;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        gameObject.SetActive(false);    // Aura defaults should set their player reference to the one found here on first disable
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
    }

    // Called by EnemyHP for quick hit stop
    //public void ActivateStop (float dur)
    //{
    //    if (!waiting)
    //    {
    //        StartCoroutine(Stop(dur));
    //    }
    //}

    // Hit stop
    IEnumerator Stop (float dur)
    {
        waiting = true;
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(dur);
        Time.timeScale = 1.0f;
        waiting = false;
    }
}
