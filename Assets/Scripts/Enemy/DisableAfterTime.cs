using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Disables object after set amount of time from activation

public class DisableAfterTime : MonoBehaviour
{
    [SerializeField] float aliveTime;
    float timeToDisable;

    void OnEnable()
    {
        timeToDisable = Time.time + aliveTime;
    }

    void Update()
    {
        if (Time.time > timeToDisable)
        {
            gameObject.SetActive(false);
        }
    }
}
