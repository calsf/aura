using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Persists for aliveTime then deactivates, while active, it damages all enemies
// Is considered ExtraDamage and separate from aura damage so that it does not delay/replace any aura damage hits

public class WildfireFlame : MonoBehaviour
{

    [SerializeField]
    Aura aura;
    int dmg;
    float deactivateTime;
    float aliveTime = 10f;
    bool hasDeactivated;
    Animator anim;
    SaveData saveData;

    public int Dmg { get { return dmg; } }

    void Awake()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        dmg = aura.baseDmg + saveData.ExtraDmg[aura.auraNumber];
    }

    void OnEnable()
    {
        anim.Play("WildfireFlameStart");
        hasDeactivated = false;
        deactivateTime = Time.time + aliveTime;
    }

    void Update()
    {
        if (!hasDeactivated && Time.time > deactivateTime)
        {
            hasDeactivated = true;
            anim.Play("WildfireFlameFadeOut");
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
