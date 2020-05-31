using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Toggles gameobject tile off and on upon entering/exiting collider this script is attached to

public class ToggleTile : MonoBehaviour
{
    [SerializeField]
    GameObject outsideTile;

    PlayerInView view;

    void Awake()
    {
        view = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInView>();
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Player")
        {
            outsideTile.SetActive(false);
            view.OutOfView = true;
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
            view.OutOfView = false;
        }
    }

}
