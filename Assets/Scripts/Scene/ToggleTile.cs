using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Toggles gameobject tile off and on upon entering/exiting collider this script is attached to

public class ToggleTile : MonoBehaviour
{
    [SerializeField]
    GameObject outsideTile;

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Player")
        {
            outsideTile.SetActive(false);
        }   
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            outsideTile.SetActive(true);
        }
    }

}
