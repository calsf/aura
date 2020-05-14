using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Toggles behaviour to shoot fixed whenever enemy is hit

public class TriggerOnHitShoot : MonoBehaviour
{
    bool onHitShoot;
    bool startingOnHit;

    // Delay between triggering shoot fixed when hit
    [SerializeField]
    float minDelay;
    [SerializeField]
    float maxDelay;
    float lastTrigger;

    ShootFixed shoot;
    Animator anim;

    PlayerInView view;
    bool playerInView;
    bool wasInView;

    void Awake()
    {
        anim = GetComponent<Animator>();
        lastTrigger = Time.time + 2f;
        view = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInView>();
        shoot = GetComponent<ShootFixed>();
    }

    void Update()
    {
        // Start toggling on hit shoot behaviour on and off once player comes into view
        playerInView = view.InView(transform);
        if (!startingOnHit && !onHitShoot && playerInView && !wasInView && Time.time > lastTrigger)
        {
            lastTrigger = Time.time + Random.Range(0, maxDelay);
        }
        wasInView = playerInView;

        if (!startingOnHit && playerInView && Time.time > lastTrigger)
        {
            startingOnHit = true;
            lastTrigger = Time.time + maxDelay;
            anim.Play("StartOnHitShoot");
        }
    }

    // Toggle onHitShoot during animation
    public void StartOnHit()
    {
        onHitShoot = true;
    }

    // Toggle onHitShoot during animation
    public void StopOnHit()
    {
        onHitShoot = false;
        lastTrigger = Time.time + (Random.Range(minDelay, maxDelay));
        startingOnHit = false;
    }

    // ShootOnHit called OnDamaged event
    public void ShootOnHit()
    {
        if (onHitShoot)
        {
            // Immediately calls shoot OnDamaged to trigger shoot fixed
            shoot.Shoot();
        }
    }
}
