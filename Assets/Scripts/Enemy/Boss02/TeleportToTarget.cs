using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToTarget : MonoBehaviour
{
    [SerializeField]
    float x;
    [SerializeField]
    float y;

    Vector2 pos;

    void Awake()
    {
        pos = new Vector2(x, y);
    }

    public void TeleportToTargetPos()
    {
        transform.position = pos;
    }
}
