using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ShootPlayer", menuName = "ShootPlayerBehaviour")]
public class ShootRotateToPlayerBehaviour : ScriptableObject
{
    public GameObject projectilePrefab;
    public float minShootDelay;     // Min delay before shooting
    public float maxShootDelay;     // Max delay before shooting
    public float shootDelay;        // General shoot delay
    public float turnSpeed;         // Speed at which object turns to face player
    public bool restrictAbove;
    public bool restrictBelow;
}
