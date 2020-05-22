using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotates an enemy, keeping the health bar at fixed position

public class RotateEnemy : MonoBehaviour
{
    [SerializeField] float rotation;
    GameObject barObject;

    void Awake()
    {
        barObject = transform.GetChild(0).gameObject;   // barObject MUST BE FIRST CHILD OF OBJECT THIS SCRIPT IS ATTACHED TO
    }

    void FixedUpdate()
    {
        // Unparent the enemy health bar and rotate enemy itself, then reattach the health bar to maintain fixed health position
        barObject.transform.parent = null;
        transform.Rotate(0, 0, rotation);
        barObject.transform.parent = transform;
        
    }
}
