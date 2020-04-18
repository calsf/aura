using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ShootFixed", menuName = "ShootFixedBehaviour")]
public class ShootFixedBehaviour : ScriptableObject
{
    public GameObject projectilePrefab;
    public float minShootDelay;     // Min delay before shooting
    public float maxShootDelay;     // Max delay before shooting
    public float shootDelay;        // General shoot delay
    public bool stopToShoot;

    public bool shootFacingDirection;   // Shoot in direction enemy is facing
    public bool shootIsChild;           // Is the object handling the shooting the enemy itself or a child of the enemy? This is for shootFacingDirection

    // Number of projectiles to shoot
    public int numProj;

    // Parrallel arrays for the x and y direction that each projectile should go, speed determined by Projectile scriptable object
    public float[] xDirection;
    public float[] yDirection;
}
