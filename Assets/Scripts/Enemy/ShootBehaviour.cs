using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootBehaviour : MonoBehaviour
{
    public abstract void StartShoot();
    public abstract void Shoot();
    public abstract void StopShooting();
}
