using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateHoming : MonoBehaviour
{
    [SerializeField]
    GameObject homingProjectile;
    [SerializeField]
    Transform spawnPos;

    public GameObject HomingProjectile { get { return homingProjectile; } }

    public void ActivateHomingProjectile()
    {
        homingProjectile.transform.position = spawnPos.transform.position;
        homingProjectile.SetActive(true);
    }
}
