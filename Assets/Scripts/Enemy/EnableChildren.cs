using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Resets all children object positions and activates them on enabling this object

public class EnableChildren : MonoBehaviour
{
    void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = Vector2.zero;
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
