using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateHoming : MonoBehaviour
{
    [SerializeField]
    GameObject homingProjectile;

    public GameObject HomingProjectile { get { return homingProjectile; } }

    public void Activate()
    {
        homingProjectile.transform.position = transform.position;
        homingProjectile.SetActive(true);
    }
}
