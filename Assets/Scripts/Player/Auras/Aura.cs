using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Aura", menuName = "Aura")]
public class Aura : ScriptableObject
{
    public string auraName;
    public int baseDmg;
    public Sprite icon;
}
