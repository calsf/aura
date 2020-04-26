using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Persists for aliveTime then deactivates, while active, it damages all enemies
// Is considered ExtraDamage and separate from aura damage so that it does not delay/replace any aura damage hits

public class WildfireFlame : MonoBehaviour
{
    int dmg = 2;
    float deactivateTime;
    float aliveTime = 5f;
    bool hasDeactivated;
    Animator anim;

    public int Dmg { get { return dmg; } }

    void Awake()
    {
        anim = GetComponent<Animator>();
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
