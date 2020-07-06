using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Aura", menuName = "Aura")]
public class Aura : ScriptableObject
{
    public string auraName;
    public int baseDmg;
    public Sprite icon;
    public int price;
    public int auraNumber;
    public int maxExtraDmg;
    public int[] enhancePrice;

    [TextArea(3, 10)]
    public string desc;
}
