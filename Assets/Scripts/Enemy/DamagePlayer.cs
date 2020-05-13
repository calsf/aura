using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DamagePlayer", menuName = "DamagePlayer")]
public class DamagePlayer : ScriptableObject
{
    public int dmg;
    public float baseSpeed;
    public bool ignoreGround;
    public bool ignoreFirstGround;
}
