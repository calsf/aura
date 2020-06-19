using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObject : MonoBehaviour
{
    [SerializeField]
    GameObject obj;

    public void Activate()
    {
        obj.SetActive(true);
    }
}
